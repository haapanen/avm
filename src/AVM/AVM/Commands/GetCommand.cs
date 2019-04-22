using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVM.Azure;
using AVM.Models;
using AVM.Options;
using AVM.Outputs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AVM.Commands
{
    public class GetCommand : ICommand
    {
        private readonly EnvironmentVariables _variables;
        private readonly GetOptions _options;
        private readonly IAzureClient _azureClient;
        private readonly IOutput _output;

        public GetCommand(EnvironmentVariables variables, GetOptions options, IAzureClient azureClient, IOutput output)
        {
            _variables = variables;
            _options = options;
            _azureClient = azureClient;
            _output = output;
        }

        public async Task<int> ExecuteAsync()
        {
            var responseText = await _azureClient.GetAsync(GetAzureUrl(_variables, _options));

            var simplifiedJson = SimplifyJson(responseText);

            _output.Write(simplifiedJson);

            return 0;
        }

        private string SimplifyJson(string responseText)
        {
            switch (_options.Type)
            {
                case AvmObjectType.BuildVariables:
                    return JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(responseText).variables);
                case AvmObjectType.ReleaseVariables:
                    var release = JsonConvert.DeserializeObject<dynamic>(responseText);

                    var output = new ReleaseVariables
                    {
                        Variables = JsonConvert.DeserializeObject<Dictionary<string, Variable>>(
                            JsonConvert.SerializeObject(release.variables)),
                        Environments =
                            JsonConvert.DeserializeObject<List<PipelineEnvironment>>(
                                JsonConvert.SerializeObject(release.environments))
                    };

                    return JsonConvert.SerializeObject(
                        output
                    );
            }

            return responseText;
        }

        private static string GetAzureUrl(EnvironmentVariables variables, GetOptions options)
        {
            string id = options.Id;
            string url = null;

            switch (options.Type)
            {
                case AvmObjectType.Build:
                case AvmObjectType.BuildVariables:
                    url =
                        $"https://dev.azure.com/{variables.Organization}/{variables.Project}/_apis/build/definitions/{id}?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    url =
                        $"https://vsrm.dev.azure.com/{variables.Organization}/{variables.Project}/_apis/release/definitions/{id}?api-version=5.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return url;
        }
    }
}
