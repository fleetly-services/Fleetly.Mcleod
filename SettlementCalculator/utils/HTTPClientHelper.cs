using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SettlementEvaluator.utils
{
    public interface IHTTPClientHelper
    {
        Task<T> GetAsync<T>(string url);
        Task<T> PostAsync<T>(string url, HttpContent contentPost);
        Task<T> PutAsync<T>(string url, HttpContent contentPut);
        Task<T> DeleteAsync<T>(string url);
    }

    public class HTTPClientHelper : IHTTPClientHelper
    {
        IHttpClientFactory httpClientFactory;
        HttpClient client;
        string ClientName;
        string BaseAddress;

        public HTTPClientHelper(IHttpClientFactory httpClientFactory, string ClientName, string BaseAddress)
        {
            this.httpClientFactory = httpClientFactory;
            this.BaseAddress = BaseAddress;
            this.ClientName = ClientName;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            T data;
            InitClient();
            try
            {
                var settings = new JsonSerializerSettings { DateFormatString = "yyyyMMddHHmmssK" };
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", "a12ed9bc-6efe-4a5a-aaad-dde97e928ad6");
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    string d = await content.ReadAsStringAsync();
                    if (d != null)
                    {
                        data = JsonConvert.DeserializeObject<T>(d, settings);
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            object o = new object();
            return (T)o;
        }

        public async Task<T> PostAsync<T>(string url, HttpContent contentPost)
        {
            T data;
            InitClient();

            try
            {
                using (HttpResponseMessage response = await client.PostAsync(url, contentPost))
                using (HttpContent content = response.Content)
                {
                    string d = await content.ReadAsStringAsync();
                    if (d != null)
                    {
                        data = JsonConvert.DeserializeObject<T>(d);
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            object o = new object();
            return (T)o;
        }

        public async Task<T> PutAsync<T>(string url, HttpContent contentPut)
        {
            T data;
            InitClient();

            try
            {
                using (HttpResponseMessage response = await client.PutAsync(url, contentPut))
                using (HttpContent content = response.Content)
                {
                    string d = await content.ReadAsStringAsync();
                    if (d != null)
                    {
                        data = JsonConvert.DeserializeObject<T>(d);
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            object o = new object();
            return (T)o;
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            T newT;
            InitClient();
            try
            {
                using (HttpResponseMessage response = await client.DeleteAsync(url))
                using (HttpContent content = response.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    if (data != null)
                    {
                        newT = JsonConvert.DeserializeObject<T>(data);
                        return newT;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            object o = new object();
            return (T)o;
        }

        private void InitClient()
        {
            client = httpClientFactory.CreateClient(ClientName);
            client.BaseAddress = new Uri(BaseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }
}
