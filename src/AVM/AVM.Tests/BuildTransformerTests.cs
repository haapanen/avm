using System;
using System.Collections.Generic;
using System.Text;
using AVM.Json;
using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AVM.Tests
{
    public class BuildTransformerTests
    {
        public JObject CreateValidPartialBuild()
        {
            var buildDefinition = new JObject();
            buildDefinition["variables"] = new JObject();
            return buildDefinition;
        }

        [Fact]
        public void UpdateBuild_ThrowsArgumentNullException_IfExistingBuildIsNull()
        {
            // Arrange
            var buildTransformer = new BuildTransformer();
            string existingBuildJson = null;
            string newVariables = "{}";

            // Act
            Assert.Throws<ArgumentNullException>(() => buildTransformer.UpdateBuild(existingBuildJson, newVariables));

            // Assert
        }

        [Fact]
        public void UpdateBuild_ThrowsArgumentNullException_IfNewVariablesIsNull()
        {
            // Arrange
            var buildTransformer = new BuildTransformer();
            string existingBuildJson = "{}";
            string newVariables = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => buildTransformer.UpdateBuild(existingBuildJson, newVariables));

            // Assert
        }

        [Fact]
        public void UpdateBuild_UpdatesVariableValue()
        {
            // Arrange
            var build = CreateValidPartialBuild();
            var variables = new Dictionary<string, Variable>
            {
                {"Variable1", new Variable {Value = "Value"}}
            };
            var buildTransformer = new BuildTransformer();

            // Act
            var output = buildTransformer.UpdateBuild(JsonConvert.SerializeObject(build), JsonConvert.SerializeObject(variables));
            var outputJObject = JsonConvert.DeserializeObject<JObject>(output);

            // Assert
            Assert.Equal("Value", outputJObject["variables"]["Variable1"].ToObject<Variable>().Value);
        }

        [Fact]
        public void UpdateBuild_UpdatesVariableIsSecret()
        {
            // Arrange
            var build = CreateValidPartialBuild();
            var variables = new Dictionary<string, Variable>
            {
                {"Variable1", new Variable {AllowOverride = true}}
            };
            var buildTransformer = new BuildTransformer();

            // Act
            var output = buildTransformer.UpdateBuild(JsonConvert.SerializeObject(build), JsonConvert.SerializeObject(variables));
            var outputJObject = JsonConvert.DeserializeObject<JObject>(output);

            // Assert
            Assert.Equal(true, outputJObject["variables"]["Variable1"].ToObject<Variable>().AllowOverride);
        }

        [Fact]
        public void UpdateBuild_UpdatesVariableAllowOverride()
        {
            // Arrange
            var build = CreateValidPartialBuild();
            var variables = new Dictionary<string, Variable>
            {
                {"Variable1", new Variable {IsSecret = true}}
            };
            var buildTransformer = new BuildTransformer();

            // Act
            var output = buildTransformer.UpdateBuild(JsonConvert.SerializeObject(build), JsonConvert.SerializeObject(variables));
            var outputJObject = JsonConvert.DeserializeObject<JObject>(output);

            // Assert
            Assert.Equal(true, outputJObject["variables"]["Variable1"].ToObject<Variable>().IsSecret);
        }
    }
}
