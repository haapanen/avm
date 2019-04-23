using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Azure
{
    public class AzureClient : IAzureClient
    {
        private readonly EnvironmentVariables _variables;
        public AzureClient(EnvironmentVariables variables)
        {
            _variables = variables;
        }

        public async Task<string> GetAsync(string url)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                });


                if (response.IsSuccessStatusCode)
                {
                    return await GetResponseAsync(response);
                }

                throw new Exception($"Loading data failed: {response.ReasonPhrase}");
            }
        }

        public async Task<string> PutAsync(string url, string content)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(url),
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });

                if (response.IsSuccessStatusCode)
                {
                    return await GetResponseAsync(response);
                }

                throw new Exception($"Storing data failed: {response.ReasonPhrase}\n{await response.Content.ReadAsStringAsync()}");
            }
        }

        private HttpClient CreateHttpClient()
        {
            var token = _variables.Token;

            return new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(
                                $":{token}")))
                }
            };
        }
        private Task<string> GetResponseAsync(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync();
        }
    }
}
