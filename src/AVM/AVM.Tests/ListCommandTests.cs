﻿using System.Threading.Tasks;
using AVM.Azure;
using AVM.Commands;
using AVM.Options;
using AVM.Outputs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace AVM.Tests
{
    public class ListCommandTests
    {
        private ListOptions CreateValidListOptions(AvmObjectType type)
        {
            return new ListOptions
            {
                Type = type
            };
        }

        private JToken CreateValidBuildsList()
        {
            return new JObject
            {
                ["count"] = 1,
                ["value"] = new JArray
                {
                    { new JObject
                    {
                        ["id"] = 1,
                        ["name"] = "Build 1",
                        ["_links"] = new JObject
                        {
                            ["web"] = new JObject
                            {
                                ["href"] = "Build 1 URL"
                            }
                        }
                    } }
                }
            };
        }

        private string CreateValidBuildsListJson()
        {
            return JsonConvert.SerializeObject(CreateValidBuildsList());
        }

        private JToken CreateValidReleasesList()
        {
            return new JObject
            {
                ["count"] = 1,
                ["value"] = new JArray
                {
                    { new JObject
                    {
                        ["id"] = 1,
                        ["name"] = "Release 1",
                        ["_links"] = new JObject
                        {
                            ["web"] = new JObject
                            {
                                ["href"] = "Release 1 URL"
                            }
                        }
                    } }
                }
            };
        }

        private string CreateValidReleasesListJson()
        {
            return JsonConvert.SerializeObject(CreateValidReleasesList());
        }

        private IOutput CreateValidOutput()
        {
            return Substitute.For<IOutput>();
        }

        private IAzureClient CreateValidAzureClient()
        {
            return Substitute.For<IAzureClient>();
        }

        private string CreateValidVariableGroupsList()
        {
            var variableGroupList = TestUtilities.CreateValidVariableGroupListJToken();
            ((JArray)variableGroupList["value"]).Add(TestUtilities.CreateValidVariableGroupJToken());
            return JsonConvert.SerializeObject(variableGroupList);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsBuilds_WhenObjectTypeIsBuild()
        {
            // Arrange
            var options = CreateValidListOptions(AvmObjectType.Build);
            var azureClient = CreateValidAzureClient();
            azureClient.GetAsync(Arg.Any<string>()).Returns(CreateValidBuildsListJson());
            var output = CreateValidOutput();
            var listCommand = new ListCommand(options, azureClient,
                output, TestUtilities.CreateValidUrlStore());

            // Act
            var response = await listCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is<string>(res => res.Contains("Build: Build 1")));
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsReleases_WhenObjectTypeIsRelease()
        {
            // Arrange
            var options = CreateValidListOptions(AvmObjectType.Release);
            var azureClient = CreateValidAzureClient();
            azureClient.GetAsync(Arg.Any<string>()).Returns(CreateValidReleasesListJson());
            var output = CreateValidOutput();
            var listCommand = new ListCommand(options, azureClient,
                output, TestUtilities.CreateValidUrlStore());

            // Act
            var response = await listCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is<string>(res => res.Contains("Release: Release 1")));
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsVariableGroups_WhenObjectTypeIsVariableGroup()
        {
            // Arrange
            var options = CreateValidListOptions(AvmObjectType.VariableGroup);
            var azureClient = CreateValidAzureClient();
            azureClient.GetAsync(Arg.Any<string>()).Returns(CreateValidVariableGroupsList());
            var output = CreateValidOutput();
            var listCommand = new ListCommand(options, azureClient,
                output, TestUtilities.CreateValidUrlStore());

            // Act
            await listCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is<string>(res => res.Contains("VariableGroup: Variable group")));
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsReleaseJson_WhenDisplayAsJsonIsOn()
        {
            
            // Arrange
            var options = CreateValidListOptions(AvmObjectType.Release);
            options.Type = AvmObjectType.Release;
            options.DisplayAsJson = true;
            var azureClient = CreateValidAzureClient();
            azureClient.GetAsync(Arg.Any<string>()).Returns(CreateValidReleasesListJson());
            var output = CreateValidOutput();
            var listCommand = new ListCommand(options, azureClient,
                output, TestUtilities.CreateValidUrlStore());

            // Act
            var response = await listCommand.ExecuteAsync();

            // Assert
            output.Received().Write(Arg.Is<string>(CreateValidReleasesListJson()));
        }
    }
}
