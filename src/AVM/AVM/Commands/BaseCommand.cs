using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AVM.Commands
{
    public class BaseCommand 
    {
        private readonly EnvironmentVariables _variables;
        protected readonly string Organization;
        protected readonly string Project;

        public BaseCommand(EnvironmentVariables variables)
        {
            _variables = variables;
            Organization = _variables.Organization;
            Project = _variables.Project;
        }

        public async Task<string> Get(string url)
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
                    return await response.Content.ReadAsStringAsync();
                }

                throw new Exception($"Storing data failed: {response.ReasonPhrase}");
            }
        }

        public async Task<string> Put(string url, string jsonContent)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(url),
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                throw new Exception($"Storing data failed: {response.ReasonPhrase}");
            }
        }

        public string PrettifyJson(string inputJson)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(inputJson), Formatting.Indented);
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
    }
}
