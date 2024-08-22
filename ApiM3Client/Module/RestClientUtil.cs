using ApiM3Connector.Module;
using ApiM3Connector.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiM3Connector.Module
{
    internal class RestClientUtil
    {
        public static string GetInputParameters(object dataObject)
        {
            if (dataObject == null)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("?");
            Type type = dataObject.GetType();

            if (dataObject is Dictionary<string, object> records)
            {
                foreach (var item in records)
                {
                    stringBuilder.Append($"{item.Key.ToString()}={(item.Value != null ? item.Value.ToString().Trim() : null)}");
                    if (item.Key != records.Last().Key)
                        stringBuilder.Append("&");
                }
            }
            else
            {
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    bool flag = true;
                    object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
                    for (int j = 0; j < customAttributes.Length; j++)
                    {
                        MapToM3 mapToM = customAttributes[j] as MapToM3;
                        if (mapToM != null)
                        {
                            flag = mapToM.Include;
                            break;
                        }
                    }

                    if (flag && propertyInfo.GetValue(dataObject, null) != null)
                    {
                        stringBuilder.Append(propertyInfo.Name + "=" + propertyInfo.GetValue(dataObject, null));
                        if (propertyInfo.Name != type.GetProperties().Last().Name)
                        {
                            stringBuilder.Append("&");
                        }
                    }
                }
            }

            return stringBuilder.ToString();
        }

        public static string GetOutputParameters(Type type)
        {
            StringBuilder stringBuilder = new StringBuilder();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                bool flag = true;
                object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
                for (int j = 0; j < customAttributes.Length; j++)
                {
                    MapToM3 mapToM = customAttributes[j] as MapToM3;
                    if (mapToM != null)
                    {
                        flag = mapToM.Include;
                        break;
                    }
                }

                if (flag)
                {
                    stringBuilder.Append(propertyInfo.Name);
                    if (propertyInfo.Name != type.GetProperties().Last().Name)
                    {
                        stringBuilder.Append(",");
                    }
                }
            }

            return stringBuilder.ToString();
        }

        public static List<string> GetProperties(Type type)
        {
            List<string> list = new List<string>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                list.Add(propertyInfo.Name);
            }

            return list;
        }
    }
}
