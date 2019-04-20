using System;
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
                result.WithNotParsed(errs =>
                    {
                        var helpText = HelpText.AutoBuild(result, h =>
                        {
                            h.AdditionalNewLineAfterOption = false;
                            return HelpText.DefaultParsingErrorsHandler(result, h);
                        }, e =>
                        {
                            return e;
                        }, true);
                        Console.WriteLine(helpText);
                    })
                .MapResult(
                    CreateCommandHandler<GetCommand, GetOptions>(),
                    CreateCommandHandler<ListCommand, ListOptions>(),
                    CreateCommandHandler<SetCommand, SetOptions>(),
                    errs =>
                    {
                        return 1;
                    }
                );
        }

        private static Func<TOptions, int> CreateCommandHandler<TCommand, TOptions>()
            where TCommand : class, ICommand
            where TOptions : class
        {
            return options =>
            {
                Services.AddSingleton<TCommand>();
                Services.AddSingleton(options);
                return Services.BuildServiceProvider().GetService<TCommand>().ExecuteAsync().Result;
            };
        }
    }
}
