using System;
using AVM.Commands;
using AVM.Options;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            Parser.Default.ParseArguments<ListBuildsOptions, GetBuildDefinitionOptions, UpdateBuildOptions>(args)
                .MapResult(
                    CreateCommandHandler<ListBuildsCommand, ListBuildsOptions>(),
                    CreateCommandHandler<GetBuildDefinitionCommand, GetBuildDefinitionOptions>(),
                    CreateCommandHandler<UpdateBuildCommand, UpdateBuildOptions>(),
 
                    errs => 1
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
