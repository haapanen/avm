using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVM.Json;
using AVM.Models;
using AVM.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Commands
{
    public class SetCommand : BaseCommand, ICommand
    {
        private readonly SetOptions _options;
        private readonly AzureBuild _azureBuild;
        private readonly AzureRelease _azureRelease;

        public SetCommand(EnvironmentVariables variables, SetOptions options, AzureRelease azureRelease, AzureBuild azureBuild) : base(variables)
        {
            _options = options;
            _azureRelease = azureRelease;
            _azureBuild = azureBuild;
        }

        public async Task<int> ExecuteAsync()
        {
            string url = null;

            switch (_options.Type)
            {
                case AvmObjectType.Build:
                case AvmObjectType.BuildVariables:
                    url = $"https://dev.azure.com/{Organization}/{Project}/_apis/build/definitions/{_options.Id}?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    url = $"https://vsrm.dev.azure.com/{Organization}/{Project}/_apis/release/definitions/{_options.Id}?api-version=5.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var existing = await Get(url);

            switch (_options.Type)
            {
                case AvmObjectType.Build:
                    break;
                case AvmObjectType.Release:
                    break;
                case AvmObjectType.BuildVariables:
                    var newVariables = await File.ReadAllTextAsync(_options.SourceFilePath,
                        Encoding.UTF8);

                    existing = _azureBuild.UpdateBuild(existing, newVariables);
                    break;
                case AvmObjectType.ReleaseVariables:
                    existing = _azureRelease.UpdateRelease(existing,
                        await File.ReadAllTextAsync(_options.SourceFilePath, Encoding.UTF8));

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var response = await Put(url, existing);

            Console.WriteLine(response);

            return 0;
        }
    }
}
