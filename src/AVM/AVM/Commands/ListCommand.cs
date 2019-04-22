using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVM.Azure;
using AVM.Options;
using AVM.Outputs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Commands
{
    public class ListCommand : ICommand
    {
        private readonly EnvironmentVariables _variables;
        private readonly ListOptions _options;
        private readonly IAzureClient _azureClient;
        private readonly IOutput _output;

        public ListCommand(EnvironmentVariables variables, ListOptions options, IAzureClient azureClient, IOutput output) 
        {
            _variables = variables;
            _options = options;
            _azureClient = azureClient;
            _output = output;
        }

        public async Task<int> ExecuteAsync()
        {
            var response = JsonConvert.DeserializeObject<JObject>(await _azureClient.GetAsync(GetUrlFor(_options, _variables)));

            _output.Write(GetOutput(response, _options));

            return 0;
        }

        private static string GetOutput(JObject response, ListOptions options)
        {
            if (options.DisplayAsJson)
            {
                return JsonConvert.SerializeObject(response);
            }

            var values = response.SelectTokens("$.value.[?(@)]");
            
            var stringBuilder = new StringBuilder();
            foreach (var value in values)
            {
                stringBuilder.Append(options.Type.ToString() + ": ");
                stringBuilder.AppendLine(value["name"].ToObject<string>());
                stringBuilder.AppendLine($" id: {value["id"]}");
                stringBuilder.AppendLine($" link: {value.SelectToken("$._links.web.href").ToObject<string>()}");
            }

            return stringBuilder.ToString();
        }

        private static string GetUrlFor(ListOptions options, EnvironmentVariables variables)
        {
            string url;

            switch (options.Type)
            {
                case AvmObjectType.Build:
                case AvmObjectType.BuildVariables:
                    url = $"https://dev.azure.com/{variables.Organization}/{variables.Project}/_apis/build/definitions?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    url = $"https://vsrm.dev.azure.com/{variables.Organization}/{variables.Project}/_apis/release/definitions?api-version=5.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return url;
        }
    }
}
