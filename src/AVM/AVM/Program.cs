using System;
using System.Collections.Generic;
using AVM.Commands;
using AVM.Options;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AVM
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static ServiceCollection Services { get; set; }
        private static readonly Dictionary<Type, Type> _optionTypesToCommandTypes = new Dictionary<Type, Type>
        {
            { typeof(GetOptions), typeof(GetCommand) },
            { typeof(SetOptions), typeof(SetCommand) },
            { typeof(ListOptions), typeof(ListCommand) },
        };
        public static void Main(string[] args)
        {
            Services = new ServiceCollection();
            Services
                .AddLogging(cfg => { cfg.AddConsole(); });

            var configurationBuilder = new ConfigurationBuilder();
            Configuration = configurationBuilder.AddEnvironmentVariables("AVM_").Build();
            var environment = new EnvironmentVariables();
            Configuration.Bind(environment);
            Services.AddSingleton(environment);

            var parser = new Parser(settings => settings.CaseInsensitiveEnumValues = true);
            var result = parser.ParseArguments<
                GetOptions, 
                ListOptions,
                SetOptions
            >(args);
            result.WithParsed(StartApp);
            result.WithNotParsed(errs =>
            {
                var helpText = HelpText.AutoBuild(result, h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e, true);
                Console.WriteLine(helpText);
            });
        }

        private static void StartApp(object optionsObject)
        {
            Services.AddSingleton(optionsObject.GetType(), optionsObject);

            if (!_optionTypesToCommandTypes.ContainsKey(optionsObject.GetType()))
            {
                Console.WriteLine("Unknown options type: {0}", optionsObject.GetType().FullName);

                return;
            }

            var commandType = _optionTypesToCommandTypes[optionsObject.GetType()];
            Services.AddSingleton(commandType);

            var command = (ICommand) Services.BuildServiceProvider().GetService(commandType);
            command.ExecuteAsync().Wait();
        }
    }
}
