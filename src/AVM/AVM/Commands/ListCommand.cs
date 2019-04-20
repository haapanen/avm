using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVM.Options;

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
                    urlPath = "build/definitions?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                    urlPath = "release/definitions?api-version=5.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var response = await Get(urlPath);

            Console.WriteLine(PrettifyJson(response));

            return 0;
        }
    }
}
