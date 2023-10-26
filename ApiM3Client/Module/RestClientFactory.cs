using ApiM3Connector.Module;
using ApiM3Connector.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApiM3Connector.Module
{

    internal class RestClientFactory
    {
        public static HttpClient CreateBasicAuthRestClient(ClientConfiguration clientConfig)
        {
            HttpClient httpClient = new HttpClient();
            byte[] bytes = Encoding.ASCII.GetBytes(clientConfig.User + ":" + clientConfig.Password);
            httpClient.BaseAddress = new Uri(clientConfig.ServiceUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(clientConfig.ContentType));
            return httpClient;
        }
    }

}
