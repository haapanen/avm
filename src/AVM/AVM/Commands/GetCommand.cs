using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVM.Models;
using AVM.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AVM.Commands
{
    public class GetCommand : BaseCommand, ICommand
    {
        private readonly GetOptions _options;

        public GetCommand(EnvironmentVariables variables, GetOptions options) : base(variables)
        {
            _options = options;
        }

        public async Task<int> ExecuteAsync()
        {
            string id = _options.Id;
            string urlPath = null;

            switch (_options.Type)
            {
                case AvmObjectType.Build:
                case AvmObjectType.BuildVariables:
                    urlPath = $"https://dev.azure.com/{Organization}/{Project}/_apis/build/definitions/{id}?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    urlPath = $"https://vsrm.dev.azure.com/{Organization}/{Project}/_apis/release/definitions/{id}?api-version=5.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var responseText = await Get(urlPath);

            string prettifiedJson = null;
            
            switch (_options.Type)
            {
                case AvmObjectType.Build:
                case AvmObjectType.Release:
                    prettifiedJson = PrettifyJson(responseText);
                    break;
                case AvmObjectType.BuildVariables:
                    prettifiedJson = JsonConvert.SerializeObject(
                        JsonConvert.DeserializeObject<dynamic>(responseText).variables,
                        Formatting.Indented);
                    break;
                case AvmObjectType.ReleaseVariables:
                    var release = JsonConvert.DeserializeObject<dynamic>(responseText);

                    var output = new ReleaseVariables
                    {
                        Variables = JsonConvert.DeserializeObject<Dictionary<string, Variable>>(JsonConvert.SerializeObject(release.variables)),
                        Environments = JsonConvert.DeserializeObject<List<PipelineEnvironment>>(JsonConvert.SerializeObject(release.environments))
                    };

                    prettifiedJson = JsonConvert.SerializeObject(
                        output, Formatting.Indented
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine(prettifiedJson);

            return 0;
        }
    }
}
