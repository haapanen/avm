using System;
using System.Collections.Generic;
using AVM.Azure;
using AVM.Commands;
using AVM.Options;
using AVM.Outputs;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AVM
{
    public class Program
    {
        private static readonly Dictionary<Type, Type> OptionTypesToCommandTypes = new Dictionary<Type, Type>
        {
            { typeof(GetOptions), typeof(GetCommand) },
            { typeof(SetOptions), typeof(SetCommand) },
            { typeof(ListOptions), typeof(ListCommand) },
        };
        public static void Main(string[] args)
        {
            var serviceCollection = CreateServiceCollection();

            var configuration = CreateConfigurationRoot();
            RegisterEnvironmentVariables(configuration, serviceCollection);
            RegisterAzureResponseHandlers(serviceCollection);
            RegisterAzureClient(serviceCollection);
            RegisterConsoleOutput(serviceCollection);
            RegisterUrlStore(serviceCollection);

            HandleArgumentParseResult(ParseArguments(args, CreateArgumentParser()), serviceCollection);
        }
        private static Parser CreateArgumentParser()
        {
            var parser = new Parser(settings => settings.CaseInsensitiveEnumValues = true);
            return parser;
        }

        private static void HandleArgumentParseResult(ParserResult<object> result, IServiceCollection serviceCollection)
        {
            result.WithParsed(StartApp(serviceCollection));
            result.WithNotParsed(HandleAppError(result));
        }

        private static ParserResult<object> ParseArguments(string[] args, Parser parser)
        {
            var result = parser.ParseArguments<
                GetOptions,
                ListOptions,
                SetOptions
            >(args);
            return result;
        }

        private static Action<IEnumerable<Error>> HandleAppError(ParserResult<object> result)
        {
            return errs =>
            {
                var helpText = HelpText.AutoBuild(result, h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e, true);
                Console.WriteLine(helpText);
            };
        }

        private static void RegisterAzureResponseHandlers(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ReleaseTransformer>();
            serviceCollection.AddSingleton<VariableContainerTransformer>();
        }

        private static void RegisterEnvironmentVariables(IConfiguration configuration, IServiceCollection serviceCollection)
        {
            var environment = new EnvironmentVariables();
            configuration.Bind(environment);
            serviceCollection.AddSingleton(environment);
        }

        private static void RegisterConsoleOutput(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IOutput, ConsoleOutput>();
        }

        private static void RegisterUrlStore(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IUrlStore, UrlStore>();
        }

        private static void RegisterAzureClient(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAzureClient, AzureClient>();
        }

        private static void RegisterOptions(IServiceCollection serviceCollection, object optionsObject)
        {
            serviceCollection.AddSingleton(optionsObject.GetType(), optionsObject);
        }

        private static void RegisterCommand(IServiceCollection serviceCollection, Type commandType)
        {
            serviceCollection.AddSingleton(commandType);
        }

        private static IConfigurationRoot CreateConfigurationRoot()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var configuration = configurationBuilder.AddEnvironmentVariables("AVM_").Build();
            return configuration;
        }

        private static IServiceCollection CreateServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            return serviceCollection;
        }

        private static Type GetCommandTypeFor(object optionsObject)
        {
            if (!OptionTypesToCommandTypes.ContainsKey(optionsObject.GetType()))
            {
                throw new ArgumentException(string.Format("Unknown options type: {0}", optionsObject.GetType().FullName));
            }

            var commandType = OptionTypesToCommandTypes[optionsObject.GetType()];
            return commandType;
        }

        private static Action<object> StartApp(IServiceCollection serviceCollection)
        {
            return optionsObject =>
            {
                RegisterOptions(serviceCollection, optionsObject);

                var commandType = GetCommandTypeFor(optionsObject);
                RegisterCommand(serviceCollection, commandType);

                var command = (ICommand)serviceCollection.BuildServiceProvider().GetService(commandType);
                command.ExecuteAsync().Wait();
            };
        }
    }
}
