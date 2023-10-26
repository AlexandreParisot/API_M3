using ApiM3Connector.Module;
using ApiM3Connector.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ApiM3Connector
{
    static public class M3Client
    {
        private static ClientConfiguration m3RestConfiguration;

        public static void  SetConnectorM3(string user, string password, string url)
        {
            m3RestConfiguration = new ClientConfiguration();
            m3RestConfiguration.ContentType = "application/json";
            m3RestConfiguration.Accept = "application/json";
            m3RestConfiguration.User = user;
            m3RestConfiguration.Password = password;
            m3RestConfiguration.ServiceUrl = url;
        }


        public static async Task<M3Response> GetDataAsync<T>(string program, string transaction, object queryParam, bool outputAllFields = false, int maxrecs = 0, bool metadata = false, bool excludempty = false)
        {
            return await Task.Run(() => { return GetData<T>(program, transaction, queryParam, outputAllFields, maxrecs, metadata, excludempty  ); });
        }


        public static M3Response GetData<T>(string program, string transaction, object queryParam, bool outputAllFields = false, int maxrecs = 0, bool metadata = false, bool excludempty = false)
        {
            if (m3RestConfiguration == null)
                return new M3Response() { Success = false, Message = "Aucun connecteur M3 instancier." };

            M3Response m3Response = new M3Response();
            try
            {
                HttpClient httpClient = RestClientFactory.CreateBasicAuthRestClient(m3RestConfiguration);
                string text = $"{program}/{transaction};metadata={metadata};maxrecs={maxrecs};excludempty={excludempty}";
                if (!outputAllFields)
                {
                    text = text + ";returncols=" + RestClientUtil.GetOutputParameters(typeof(T));
                }

                text = text + " " + RestClientUtil.GetInputParameters(queryParam);
                HttpResponseMessage result = httpClient.GetAsync(text).Result;
                if (!result.IsSuccessStatusCode)
                {
                    m3Response.Success = false;
                    m3Response.Message = program + "/" + transaction + ": M3 appel en erreur.";
                    return m3Response;
                }

                string result2 = result.Content.ReadAsStringAsync().Result;
                string m3ErrorMessage = GetM3ErrorMessage(result2);
                if (!string.IsNullOrEmpty(m3ErrorMessage))
                {
                    m3Response.Success = false;
                    m3Response.Message = program + "/" + transaction + ": " + m3ErrorMessage;
                    return m3Response;
                }

                dynamic val = JObject.Parse(result2);

                m3Response.DataRaw = val.ToString();
                m3Response.Data = (object)GetValueListFromM3ResultByMultipleSelectors<T>(val, RestClientUtil.GetProperties(typeof(T)), metadata);
                m3Response.M3Records = GetValueDictionaryFromM3Result(val,metadata);

                return m3Response;
            }
            catch (Exception ex)
            {
                m3Response.Success = false;
                m3Response.Message = program + "/" + transaction + $": une erreur s'est produite. {ex.Message}";
                return m3Response;
            }
        }


        private static string ResponseAsText(JObject jObject)
        {
            string text = "";
            if (jObject == null)
            {
                return text = "Reponse est null";
            }

            int num = jObject.Children().Count();
            if (num == 4)
            {
                if (!Regex.IsMatch(jObject.Children().ToList()[3].ToString(), "^\\d+$"))
                {
                    return jObject.ToString();
                }

                try
                {
                    string text2 = string.Empty;
                    for (int i = 0; i < num; i++)
                    {
                        text2 += jObject.Children().ToList()[i].ToString().Remove(0, 12);
                    }

                    return Regex.Replace(text2, "[^a-zA-Z0-9_.]+", " ", RegexOptions.Compiled);
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }

            return jObject.ToString();
        }


        private static Dictionary<string, string> GetValueDictionaryFromM3Result(JObject jObject, bool metadata)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                int index = (metadata ? 3 : 2);
                JsonParent[] array = JsonConvert.DeserializeObject<JsonParent[]>(jObject.Children().ToList()[index].ToString().Remove(0, 12));
                new List<object>();
                foreach (JsonParent jsonParent in array)
                {
                    if (jsonParent.NameValue != null)
                    {
                        foreach (var jsonChild in jsonParent.NameValue)
                        {
                            if (jsonChild != null)
                            {
                                try
                                {
                                    if(!dictionary.ContainsKey(jsonChild.Name.ToString()))
                                        dictionary.Add(jsonChild.Name.ToString(), jsonChild.Value.ToString());
                                    else
                                        dictionary[jsonChild.Name.ToString()] = jsonChild.Value.ToString();
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(ex.Message);
                                }
                            }
                        }
                    }
                    else if (jsonParent.RowIndex != null)
                    {
                        if (!dictionary.ContainsKey(nameof(jsonParent.RowIndex)))
                            dictionary.Add(nameof(jsonParent.RowIndex), jsonParent.RowIndex.ToString());
                    }

                }
                return dictionary;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        private static List<T> GetValueListFromM3ResultByMultipleSelectors<T>(JObject jObject, IList<string> keys, bool metadata)
        {
            List<T> list = new List<T>();
            try
            {
                int index = (metadata ? 3 : 2);
                JsonParent[] array = JsonConvert.DeserializeObject<JsonParent[]>(jObject.Children().ToList()[index].ToString().Remove(0, 12));
                new List<object>();
                foreach (JsonParent jsonParent in array)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string key in keys)
                    {
                        JsonChild jsonChild = jsonParent.NameValue.Where((JsonChild a) => a.Name.Equals(key)).FirstOrDefault();
                        if (jsonChild != null)
                        {
                            dictionary.Add(key, jsonChild.Value.TrimEnd(Array.Empty<char>()));
                        }
                    }

                    T item = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(dictionary));
                    list.Add(item);
                    dictionary.Clear();
                }

                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetM3ErrorMessage(string text)
        {
            string result = null;
            try
            {
                if (text == null)
                {
                    return "Pas de reponse depuis M3";
                }

                dynamic val = JObject.Parse(text);
                if (val == null)
                {
                    return "Pas de reponse depuis M3";
                }

                dynamic val2 = val.Property("MIRecord");
                if (val2 == null && text.Contains("NOK"))
                {
                    return Regex.Split(text, "NOK")[1];
                }

                if (val2 == null && text.Contains("Message"))
                {
                    return Regex.Split(text, "Message")[1];
                }

                return result;
            }
            catch (Exception ex)
            {
                return $"Erreur pendant la réponse depuis M3. {ex.Message}";
            }
        }
    }
}