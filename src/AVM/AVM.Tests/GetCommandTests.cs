using System.Collections.Generic;
using System.Threading.Tasks;
using AVM.Azure;
using AVM.Commands;
using AVM.Models;
using AVM.Options;
using AVM.Outputs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace AVM.Tests
{
    public class GetCommandTests
    {
        private string CreateValidBuildJson()
        {
            return JsonConvert.SerializeObject(TestUtilities.CreateValidBuild());
        }

        private string CreateValidReleaseJson()
        {
            return JsonConvert.SerializeObject(TestUtilities.CreateValidRelease());
        }

        private void SetupDefaultGetAsync(IAzureClient azureClient)
        {
            azureClient.GetAsync(Arg.Any<string>())
                .Returns(Task.FromResult(CreateValidBuildJson()));
        }

        private IAzureClient CreateValidAzureClient()
        {
            var azureClient = Substitute.For<IAzureClient>();

            SetupDefaultGetAsync(azureClient);

            return azureClient;
        }

        private GetOptions CreateValidGetOptions()
        {
            return new GetOptions
            {
                Type = AvmObjectType.Build,
                Id = "1"
            };
        }

        private IOutput CreateValidOutput()
        {
            return Substitute.For<IOutput>();
        }

        private string Prettify(string json)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
        }

        private string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        [Fact]
        public async Task ExecuteAsync_OutputsBuild_WhenGettingBuild()
        {
            // Arrange
            var output = CreateValidOutput();
            var options = CreateValidGetOptions();
            options.Type = AvmObjectType.Build;
            var azureClient = CreateValidAzureClient();
            azureClient
                .GetAsync(Arg.Any<string>()).Returns(Task.FromResult(CreateValidBuildJson()));
            var getCommand = new GetCommand(options, azureClient, output, TestUtilities.CreateValidUrlStore());

            // Act 
            await getCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is(CreateValidBuildJson()));
        }

        [Fact]
        public async Task ExecuteAsync_OutputsRelease_WhenGettingRelease()
        {
            // Arrange
            var output = CreateValidOutput();
            var options = CreateValidGetOptions();
            options.Type = AvmObjectType.Release;
            var azureClient = CreateValidAzureClient();
            azureClient
                .GetAsync(Arg.Any<string>()).Returns(Task.FromResult(CreateValidReleaseJson()));
            var getCommand = new GetCommand(options, azureClient, output, TestUtilities.CreateValidUrlStore());

            // Act 
            await getCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is(CreateValidReleaseJson()));
        }

        [Fact]
        public async Task ExecuteAsync_OutputsBuildVariables_WhenGettingBuildVariables()
        {
            // Arrange
            var build = TestUtilities.CreateValidBuild();
            build["variables"]["Variable"] = TestUtilities.CreateValidVariableJObject();
            var output = CreateValidOutput();
            var options = CreateValidGetOptions();
            options.Type = AvmObjectType.BuildVariables;
            var azureClient = CreateValidAzureClient();
            azureClient
                .GetAsync(Arg.Any<string>()).Returns(Task.FromResult(Serialize(build)));
            var getCommand = new GetCommand(options, azureClient, output, TestUtilities.CreateValidUrlStore());

            // Act 
            await getCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is(Serialize(build["variables"])));
        }

        [Fact]
        public async Task ExecuteAsync_OutputsReleaseVariables_WhenGettingReleaseVariables()
        {
            // Arrange
            string variableName = "Variable";
            var release = TestUtilities.CreateValidRelease();
            release["variables"][variableName] = TestUtilities.CreateValidVariableJObject();
            var env = TestUtilities.CreateValidReleaseEnvironment();
            env["variables"][variableName] = TestUtilities.CreateValidVariableJObject();
            (release["environments"] as JArray).Add(env);
            var output = CreateValidOutput();
            var options = CreateValidGetOptions();
            options.Type = AvmObjectType.ReleaseVariables;
            var azureClient = CreateValidAzureClient();
            azureClient
                .GetAsync(Arg.Any<string>()).Returns(Task.FromResult(Serialize(release)));
            var getCommand = new GetCommand(options, azureClient, output, TestUtilities.CreateValidUrlStore());

            // Act
            await getCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is(Serialize(new ReleaseVariables
            {
                Variables = new Dictionary<string, Variable>
                {
                    { "Variable", CreateDefaultVariable() }
                },
                Environments = new List<PipelineEnvironment>
                {
                    new PipelineEnvironment
                    {
                        Id = env["id"].Value<int>(),
                        Name = env["name"].Value<string>(),
                        Variables = new Dictionary<string, Variable>
                        {
                            { "Variable", CreateDefaultVariable() }
                        }
                    }
                }
            })));
        }

        private static Variable CreateDefaultVariable()
        {
            return new Variable
            {
                Value = "value",
                AllowOverride = false,
                IsSecret = false
            };
        }
    }
}
