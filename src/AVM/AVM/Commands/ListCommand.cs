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
        private readonly ListOptions _options;
        private readonly IAzureClient _azureClient;
        private readonly IOutput _output;
        private readonly IUrlStore _urlStore;

        public ListCommand(ListOptions options, IAzureClient azureClient, IOutput output, IUrlStore urlStore) 
        {
            _options = options;
            _azureClient = azureClient;
            _output = output;
            _urlStore = urlStore;
        }

        public async Task<int> ExecuteAsync()
        {
            var response = JsonConvert.DeserializeObject<JObject>(await _azureClient.GetAsync(_urlStore.GetListUrl(_options.Type)));

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
                stringBuilder.Append(options.Type + ": ");
                stringBuilder.AppendLine(value["name"].ToObject<string>());
                stringBuilder.AppendLine($" id: {value["id"]}");
                stringBuilder.AppendLine($" link: {value.SelectToken("$._links.web.href")?.ToObject<string>() ?? "no link available"}");
            }

            return stringBuilder.ToString();
        }
    }
}
