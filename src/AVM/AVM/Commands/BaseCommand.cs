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
        private readonly string _organization;
        private readonly string _project;

        public BaseCommand(EnvironmentVariables variables)
        {
            _variables = variables;
            _organization = _variables.Organization;
            _project = _variables.Project;
        }

        public async Task<string> Get(string urlPath)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://dev.azure.com/{_organization}/{_project}/_apis/" + urlPath)
                });

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> Put(string urlPath, string jsonContent)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"https://dev.azure.com/{_organization}/{_project}/_apis/" + urlPath),
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                });

                return await response.Content.ReadAsStringAsync();
            }
        }

        public string PrettifyJson(string inputJson)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(inputJson), Formatting.Indented);
        }

        private HttpClient CreateHttpClient()
        {
            var token = File.ReadAllText(_variables.TokenPath);

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
