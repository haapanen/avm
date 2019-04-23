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
        private readonly GetOptions _options;
        private readonly IAzureClient _azureClient;
        private readonly IOutput _output;
        private readonly IUrlStore _urlStore;

        public GetCommand(GetOptions options, IAzureClient azureClient, IOutput output, IUrlStore urlStore)
        {
            _options = options;
            _azureClient = azureClient;
            _output = output;
            _urlStore = urlStore;
        }

        public async Task<int> ExecuteAsync()
        {
            var responseText = await _azureClient.GetAsync(_urlStore.GetObjectUrl(_options.Type, _options.Id));

            var simplifiedJson = SimplifyJson(responseText);

            _output.Write(simplifiedJson);

            return 0;
        }

        private string SimplifyJson(string responseText)
        {
            switch (_options.Type)
            {
                case AvmObjectType.BuildVariables:
                case AvmObjectType.VariableGroupVariables:
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
    }
}
