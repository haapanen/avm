using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using AVM.Azure;
using Xunit;

namespace AVM.Tests
{
    public class ReleaseTransformerTests
    {
        private JObject CreatePartiallyValidRelease()
        {
            var release = new JObject();
            release["variables"] = new JObject();
            release["environments"] = new JArray();
            return release;
        }

        [Fact]
        public void UpdateRelease_ThrowsArgumentNullException_IfExistingReleaseJsonIsEmpty()
        {
            // Arrange
            string existingJson = null;
            var newValuesJson = "{}";
            var releaseTransformer = new ReleaseTransformer();

            // Act
            Assert.Throws<ArgumentNullException>(() => releaseTransformer.UpdateRelease(existingJson, newValuesJson));

            // Assert
        }

        [Fact]
        public void UpdateRelease_ThrowsArgumentNullException_IfNewValuesReleaseJsonIsEmpty()
        {
            // Arrange
            string existingJson = "{}";
            string newValuesJson = null;
            var releaseTransformer = new ReleaseTransformer();

            // Act
            Assert.Throws<ArgumentNullException>(() => releaseTransformer.UpdateRelease(existingJson, newValuesJson));

            // Assert
        }

        [Fact]
        public void UpdateRelease_SetsVariableValueToNull_IfNewValueIsNull()
        {
            // Arrange
            string variableName = "variable1";
            var existing = CreatePartiallyValidRelease();
            existing["variables"][variableName] = new JObject();
            existing["variables"][variableName]["value"] = "value";
            var newRelease = new ReleaseVariables
            {
                Variables = new Dictionary<string, Variable>
                {
                    {variableName, null}
                }
            };

            // Act
            var output = JsonConvert.DeserializeObject<JObject>(new ReleaseTransformer().UpdateRelease(JsonConvert.SerializeObject(existing),
                JsonConvert.SerializeObject(newRelease)));

            // Assert
            Assert.Null(output["variables"][variableName].ToObject(typeof(object)));
        }

        [Fact]
        public void UpdateRelease_SetsVariableValue()
        {
            // Arrange
            string variableName = "variable1";
            var existing = CreatePartiallyValidRelease();
            existing["variables"][variableName] = new JObject();
            var newRelease = new ReleaseVariables
            {
                Variables = new Dictionary<string, Variable>
                {
                    {variableName, new Variable
                    {
                        Value = "New value"
                    }}
                }
            };

            // Act
            var output = JsonConvert.DeserializeObject<JObject>(new ReleaseTransformer().UpdateRelease(JsonConvert.SerializeObject(existing),
                JsonConvert.SerializeObject(newRelease)));

            // Assert
            Assert.Equal("New value", output["variables"][variableName]["value"]);
        }

        [Fact]
        public void UpdateRelease_SetsVariableIsSecret()
        {
            // Arrange
            string variableName = "variable1";
            var existing = CreatePartiallyValidRelease();
            existing["variables"][variableName] = new JObject();
            var newRelease = new ReleaseVariables
            {
                Variables = new Dictionary<string, Variable>
                {
                    {variableName, new Variable
                    {
                        IsSecret = true
                    }}
                }
            };

            // Act
            var output = JsonConvert.DeserializeObject<JObject>(new ReleaseTransformer().UpdateRelease(JsonConvert.SerializeObject(existing),
                JsonConvert.SerializeObject(newRelease)));

            // Assert
            Assert.Equal(true, output["variables"][variableName]["isSecret"]);
        }

        [Fact]
        public void UpdateRelease_SetsVariableAllowOverride()
        {
            // Arrange
            string variableName = "variable1";
            var existing = CreatePartiallyValidRelease();
            existing["variables"][variableName] = new JObject();
            var newRelease = new ReleaseVariables
            {
                Variables = new Dictionary<string, Variable>
                {
                    {variableName, new Variable
                    {
                        AllowOverride = true
                    }}
                }
            };

            // Act
            var output = JsonConvert.DeserializeObject<JObject>(new ReleaseTransformer().UpdateRelease(JsonConvert.SerializeObject(existing),
                JsonConvert.SerializeObject(newRelease)));

            // Assert
            Assert.Equal(true, output["variables"][variableName]["allowOverride"]);
        }

        [Fact]
        public void UpdateRelease_ThrowsArgumentException_WhenEnvironmentDoesntExist()
        {
            // Arrange
            var release = CreatePartiallyValidRelease();
            var releaseVariables = new ReleaseVariables
            {
                Variables = new Dictionary<string, Variable>(),
                Environments = new List<PipelineEnvironment>
                {
                    new PipelineEnvironment
                    {
                        Id = 1
                    }
                }
            };
            var releaseTransformer = new ReleaseTransformer();

            // Act
            Assert.Throws<ArgumentException>(() => releaseTransformer.UpdateRelease(JsonConvert.SerializeObject(release),
                JsonConvert.SerializeObject(releaseVariables)));

            // Assert
        }

        [Fact]
        public void UpdateRelease_UpdatesCorrectEnvironment()
        {
            // Arrange
            var release = CreatePartiallyValidRelease();
            var envArray = new JArray();
            var env1 = new JObject();
            env1["id"] = 1;
            env1["variables"] = new JObject();
            var env2 = new JObject();
            env2["id"] = 2;
            env2["variables"] = new JObject();
            envArray.Add(env1);
            envArray.Add(env2);
            release["environments"] = envArray;
            var releaseVariables = new ReleaseVariables
            {
                Variables = new Dictionary<string, Variable>(),
                Environments = new List<PipelineEnvironment>
                {
                    new PipelineEnvironment
                    {
                        Id = 2,
                        Variables = new Dictionary<string, Variable>
                        {
                            { "Variable1", new Variable
                            {
                                Value = "Value"
                            } }
                        }
                    }
                }
            };
            var releaseTransformer = new ReleaseTransformer();

            // Act
            var output = releaseTransformer.UpdateRelease(JsonConvert.SerializeObject(release),
                JsonConvert.SerializeObject(releaseVariables));

            // Assert
            Assert.Equal("Value", JsonConvert.DeserializeObject<JObject>(output)
                .SelectToken($"$.environments[?(@.id == 2)].variables")["Variable1"]
                .ToObject<Variable>()
                .Value);
        }
    }
}
