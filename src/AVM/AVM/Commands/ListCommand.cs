using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVM.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Commands
{
    public class ListCommand : BaseCommand, ICommand
    {
        private readonly ListOptions _options;

        public ListCommand(EnvironmentVariables variables, ListOptions options) : base(variables)
        {
            _options = options;
        }

        public async Task<int> ExecuteAsync()
        {
            string urlPath;

            switch (_options.Type)
            {
                case AvmObjectType.Build:
                    urlPath = $"https://dev.azure.com/{Organization}/{Project}/_apis/build/definitions?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                    urlPath = $"https://vsrm.dev.azure.com/{Organization}/{Project}/_apis/release/definitions?api-version=5.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var response = await Get(urlPath);
            var responseJson = JsonConvert.DeserializeObject<JObject>(response);
            var values = responseJson.SelectTokens("$.value.[?(@)]");

            foreach (var value in values)
            {
                Console.Write(_options.Type.ToString() + ": ");
                Console.WriteLine(value["name"]);
                Console.WriteLine($" id: {value["id"]}");
                Console.WriteLine($" link: {value.SelectToken("$._links.web.href").ToObject<string>()}");
            }

            return 0;
        }
    }
}
