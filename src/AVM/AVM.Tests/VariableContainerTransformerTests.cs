using System;
using System.Collections.Generic;
using AVM.Azure;
using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AVM.Tests
{
    public class VariableContainerTransformerTests
    {
        public JObject CreateValidPartialBuild()
        {
            var buildDefinition = new JObject();
            buildDefinition["variables"] = new JObject();
            return buildDefinition;
        }

        [Fact]
        public void UpdateContainer_ThrowsArgumentNullException_IfExistingBuildIsNull()
        {
            // Arrange
            var transformer = new VariableContainerTransformer();
            string existingContainerJson = null;
            var newVariables = "{}";

            // Act
            Assert.Throws<ArgumentNullException>(() => transformer.UpdateContainer(existingContainerJson, newVariables));

            // Assert
        }

        [Fact]
        public void UpdateContainer_ThrowsArgumentNullException_IfNewVariablesIsNull()
        {
            // Arrange
            var transformer = new VariableContainerTransformer();
            var existingContainerJson = "{}";
            string newVariables = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => transformer.UpdateContainer(existingContainerJson, newVariables));

            // Assert
        }

        [Fact]
        public void UpdateContainer_UpdatesVariableValue()
        {
            // Arrange
            var build = CreateValidPartialBuild();
            var variables = new Dictionary<string, Variable>
            {
                {"Variable1", new Variable {Value = "Value"}}
            };
            var transformer = new VariableContainerTransformer();

            // Act
            var output = transformer.UpdateContainer(JsonConvert.SerializeObject(build), JsonConvert.SerializeObject(variables));
            var outputJObject = JsonConvert.DeserializeObject<JObject>(output);

            // Assert
            Assert.Equal("Value", outputJObject["variables"]["Variable1"].ToObject<Variable>().Value);
        }

        [Fact]
        public void UpdateContainer_UpdatesVariableIsSecret()
        {
            // Arrange
            var build = CreateValidPartialBuild();
            var variables = new Dictionary<string, Variable>
            {
                {"Variable1", new Variable {AllowOverride = true}}
            };
            var transformer = new VariableContainerTransformer();

            // Act
            var output = transformer.UpdateContainer(JsonConvert.SerializeObject(build), JsonConvert.SerializeObject(variables));
            var outputJObject = JsonConvert.DeserializeObject<JObject>(output);

            // Assert
            Assert.Equal(true, outputJObject["variables"]["Variable1"].ToObject<Variable>().AllowOverride);
        }

        [Fact]
        public void UpdateContainer_UpdatesVariableAllowOverride()
        {
            // Arrange
            var build = CreateValidPartialBuild();
            var variables = new Dictionary<string, Variable>
            {
                {"Variable1", new Variable {IsSecret = true}}
            };
            var transformer = new VariableContainerTransformer();

            // Act
            var output = transformer.UpdateContainer(JsonConvert.SerializeObject(build), JsonConvert.SerializeObject(variables));
            var outputJObject = JsonConvert.DeserializeObject<JObject>(output);

            // Assert
            Assert.Equal(true, outputJObject["variables"]["Variable1"].ToObject<Variable>().IsSecret);
        }
    }
}
