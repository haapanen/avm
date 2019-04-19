using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AVM.Options;
using Newtonsoft.Json;

namespace AVM.Commands
{
    public class ListBuildsCommand : BaseCommand, ICommand
    {
        private readonly EnvironmentVariables _environmentVariables;

        public ListBuildsCommand(ListBuildsOptions options, EnvironmentVariables environmentVariables) : base(environmentVariables)
        {
            _environmentVariables = environmentVariables;
        }

        public async Task<int> ExecuteAsync()
        {
            var buildsJson = await Get("build/definitions?api-version=5.0");

            Console.WriteLine(PrettifyJson(buildsJson));

            return 0;
        }
    }
}
