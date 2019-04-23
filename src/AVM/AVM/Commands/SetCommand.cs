using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AVM.Azure;
using AVM.Options;
using AVM.Outputs;

namespace AVM.Commands
{
    public class SetCommand : ICommand
    {
        private readonly SetOptions _options;
        private readonly VariableContainerTransformer _variableContainerTransformer;
        private readonly ReleaseTransformer _releaseTransformer;
        private readonly IUrlStore _urlStore;
        private readonly IAzureClient _azureClient;
        private readonly IOutput _output;

        public SetCommand(SetOptions options, ReleaseTransformer releaseTransformer, VariableContainerTransformer variableContainerTransformer, IUrlStore urlStore, IAzureClient azureClient, IOutput output) 
        {
            _options = options;
            _releaseTransformer = releaseTransformer;
            _variableContainerTransformer = variableContainerTransformer;
            _urlStore = urlStore;
            _azureClient = azureClient;
            _output = output;
        }

        public async Task<int> ExecuteAsync()
        {
            var url = _urlStore.GetObjectUrl(_options.Type, _options.Id);

            var existing = await _azureClient.GetAsync(_urlStore.GetObjectUrl(_options.Type, _options.Id));

            var response = await _azureClient.PutAsync(url, await TransformObject(existing));

            _output.Write(response);

            return 0;
        }

        private async Task<string> TransformObject(string existing)
        {
            string newValue = existing;
            switch (_options.Type)
            {
                case AvmObjectType.Build:
                    break;
                case AvmObjectType.Release:
                    break;
                case AvmObjectType.BuildVariables:
                case AvmObjectType.VariableGroupVariables:
                    var newVariables = await File.ReadAllTextAsync(_options.SourceFilePath,
                        Encoding.UTF8);

                    newValue = _variableContainerTransformer.UpdateContainer(existing, newVariables);
                    break;
                case AvmObjectType.ReleaseVariables:
                    newValue = _releaseTransformer.UpdateRelease(existing,
                        await File.ReadAllTextAsync(_options.SourceFilePath, Encoding.UTF8));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return newValue;
        }
    }
}
