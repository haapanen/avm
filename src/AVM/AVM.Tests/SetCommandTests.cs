using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    public class SetCommandTests
    {
        private SetOptions CreateValidSetOptions(AvmObjectType type)
        {
            return new SetOptions
            {
                Id = "1",
                Type = type,
                SourceFilePath = "source.json"
            };
        }

        private IUrlStore CreateValidUrlStore()
        {
            return Substitute.For<IUrlStore>();
        }

        private IOutput CreateValidOutput()
        {
            return Substitute.For<IOutput>();
        }

        private IAzureClient CreateValidAzureClient()
        {
            return Substitute.For<IAzureClient>();
        }

        [Fact]
        public async Task ExecuteAsync_UploadsCorrectlyModifiedBuild()
        {
            // Arrange
            var build = TestUtilities.CreateValidBuild();
            var expectedBuild = TestUtilities.CreateValidBuild();
            expectedBuild["variables"]["Variable"] = TestUtilities.CreateValidVariableJObject();
            var variables = new Dictionary<string, Variable>
            {
                { "Variable", TestUtilities.CreateValidVariable() }
            };
            var azureClient = CreateValidAzureClient();
            azureClient.GetAsync(Arg.Any<string>()).Returns(JsonConvert.SerializeObject(build));
            var output = CreateValidOutput();
            var options = CreateValidSetOptions(AvmObjectType.BuildVariables);
            File.WriteAllText(options.SourceFilePath, JsonConvert.SerializeObject(variables));
            var setCommand = new SetCommand(options, new ReleaseTransformer(),
                new VariableContainerTransformer(), CreateValidUrlStore(), azureClient, output);

            // Act
            await setCommand.ExecuteAsync();
            File.Delete(options.SourceFilePath);

            // Assert
            await azureClient.Received().PutAsync(Arg.Any<string>(), Arg.Is<string>(res => JToken.DeepEquals(JsonConvert.DeserializeObject<JToken>(res), expectedBuild)));
        }

        [Fact]
        public async Task ExecuteAsync_UploadsCorrectlyModifiedRelease()
        {
            // Arrange
            var release = TestUtilities.CreateValidRelease();
            var env = TestUtilities.CreateValidReleaseEnvironment();
            ((JArray)release["environments"]).Add(env);

            var expectedRelease = TestUtilities.CreateValidRelease();
            expectedRelease["variables"]["Variable"] = TestUtilities.CreateValidVariableJObject();
            env = TestUtilities.CreateValidReleaseEnvironment();
            ((JArray)expectedRelease["environments"]).Add(env);
            env["variables"]["Variable"] = TestUtilities.CreateValidVariableJObject();

            var variablesFileContent = new ReleaseVariables
            {
                Environments = new List<PipelineEnvironment>
                {
                    new PipelineEnvironment
                    {
                        Id = 1,
                        Name = "Release 1",
                        Variables = new Dictionary<string, Variable>
                        {
                            {"Variable", TestUtilities.CreateValidVariable()}
                        }
                    }
                },
                Variables = new Dictionary<string, Variable>
                {
                    { "Variable", TestUtilities.CreateValidVariable() }
                }
            };

            var options = CreateValidSetOptions(AvmObjectType.ReleaseVariables);
            File.WriteAllText(options.SourceFilePath, JsonConvert.SerializeObject(variablesFileContent));
            var azureClient = CreateValidAzureClient();
            azureClient.GetAsync(Arg.Any<string>()).Returns(JsonConvert.SerializeObject(release));
            var output = CreateValidOutput();
            var urlStore = CreateValidUrlStore();
            var setCommand = new SetCommand(options, new ReleaseTransformer(), new VariableContainerTransformer(), urlStore, azureClient, output);

            // Act
            await setCommand.ExecuteAsync();
            File.Delete(options.SourceFilePath);

            // Assert
            await azureClient.Received().PutAsync(Arg.Any<string>(),
                Arg.Is<string>(res => JToken.DeepEquals(JsonConvert.DeserializeObject<JToken>(res), expectedRelease)));
        }

        [Fact]
        public async Task ExecuteAsync_UploadsCorrectlyModifiedVariableGroup()
        {
            // Arrange
            var variableGroup = TestUtilities.CreateValidVariableGroupJToken();
            var expectedVariableGroup = TestUtilities.CreateValidVariableGroupJToken();
            expectedVariableGroup["variables"]["Variable"] = TestUtilities.CreateValidVariableJObject();
            var variables = new Dictionary<string, Variable>
            {
                { "Variable", TestUtilities.CreateValidVariable() }
            };
            var azureClient = CreateValidAzureClient();
            azureClient.GetAsync(Arg.Any<string>()).Returns(JsonConvert.SerializeObject(variableGroup));
            var output = CreateValidOutput();
            var options = CreateValidSetOptions(AvmObjectType.VariableGroupVariables);
            File.WriteAllText(options.SourceFilePath, JsonConvert.SerializeObject(variables));
            var setCommand = new SetCommand(options, new ReleaseTransformer(),
                new VariableContainerTransformer(), CreateValidUrlStore(), azureClient, output);

            // Act
            await setCommand.ExecuteAsync();
            File.Delete(options.SourceFilePath);

            // Assert
            await azureClient.Received().PutAsync(Arg.Any<string>(), Arg.Is<string>(res => JToken.DeepEquals(JsonConvert.DeserializeObject<JToken>(res), expectedVariableGroup)));
        }

        [Fact]
        public async Task ExecuteAsync_UploadsBuild()
        {
            // Arrange
            var options = CreateValidSetOptions(AvmObjectType.Build);
            var build = TestUtilities.CreateValidBuild();
            File.WriteAllText(options.SourceFilePath, JsonConvert.SerializeObject(build));

            var azureClient = CreateValidAzureClient();
            var output = CreateValidOutput();

            var setCommand = new SetCommand(options, new ReleaseTransformer(),
                new VariableContainerTransformer(), CreateValidUrlStore(), azureClient, output);

            // Act
            await setCommand.ExecuteAsync();
            File.Delete(options.SourceFilePath);

            // Assert
            await azureClient.PutAsync(Arg.Any<string>(),
                Arg.Is<string>(res => JToken.DeepEquals(JsonConvert.DeserializeObject<JToken>(res), build)));
        }

        [Fact]
        public async Task ExecuteAsync_UploadsRelease()
        {
            // Arrange
            var options = CreateValidSetOptions(AvmObjectType.Release);
            var release = TestUtilities.CreateValidRelease();
            File.WriteAllText(options.SourceFilePath, JsonConvert.SerializeObject(release));

            var azureClient = CreateValidAzureClient();
            var output = CreateValidOutput();

            var setCommand = new SetCommand(options, new ReleaseTransformer(),
                new VariableContainerTransformer(), CreateValidUrlStore(), azureClient, output);

            // Act
            await setCommand.ExecuteAsync();
            File.Delete(options.SourceFilePath);

            // Assert
            await azureClient.PutAsync(Arg.Any<string>(),
                Arg.Is<string>(res => JToken.DeepEquals(JsonConvert.DeserializeObject<JToken>(res), release)));
        }
    }
}
