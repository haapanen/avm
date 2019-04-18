using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AVM.Options;
using Newtonsoft.Json;

namespace AVM.Commands
{
    public class UpdateBuildCommand : ICommand
    {
        private readonly UpdateBuildOptions _options;
        private readonly EnvironmentVariables _environment;

        public UpdateBuildCommand(UpdateBuildOptions options, EnvironmentVariables environment)
        {
            _options = options;
            _environment = environment;
        }

        public async Task<int> ExecuteAsync()
        {
            var token = File.ReadAllText(_environment.TokenPath);
            var organization = _environment.Organization;
            var project = _environment.Project;
            var definitionId = _options.DefinitionId;
            var source = File.ReadAllText(_options.SourceFile);

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
                            $"https://dev.azure.com/{organization}/{project}/_apis/build/definitions/{definitionId}?api-version=5.0"),
                    Method = HttpMethod.Put,
                    Content = new StringContent(source, Encoding.UTF8, "application/json"),
                });
                var json = await result.Content.ReadAsStringAsync();

                Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented));
            }

            return 0;
        }
    }
}