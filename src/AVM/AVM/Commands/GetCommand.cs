using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVM.Options;
using Newtonsoft.Json;

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
                    urlPath = $"build/definitions/{id}?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    urlPath = $"release/definitions/{id}?api-version=5.0";
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine(prettifiedJson);

            return 0;
        }
    }
}
