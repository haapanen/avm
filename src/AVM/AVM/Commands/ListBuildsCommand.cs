using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AVM.Options;
using Newtonsoft.Json;

namespace AVM.Commands
{
    public class ListBuildsCommand : ICommand
    {
        private readonly EnvironmentVariables _environmentVariables;

        public ListBuildsCommand(ListBuildsOptions options, EnvironmentVariables environmentVariables)
        {
            _environmentVariables = environmentVariables;
        }

        public async Task<int> ExecuteAsync()
        {
            var token = File.ReadAllText(_environmentVariables.TokenPath);
            var organization = _environmentVariables.Organization;
            var project = _environmentVariables.Project;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "", token))));

                var result = await client.SendAsync(new HttpRequestMessage
                {
                    RequestUri =
                        new Uri(
                            $"https://dev.azure.com/{organization}/{project}/_apis/build/definitions?api-version=5.0"),
                });
                var json = await result.Content.ReadAsStringAsync();

                Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented));
            }

            return 0;
        }
    }
}
