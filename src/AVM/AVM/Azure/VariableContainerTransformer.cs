using System;
using System.Collections.Generic;
using AVM.Extensions;
using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Azure
{
    public class VariableContainerTransformer
    {
        public string UpdateContainer(string existingContainerJson, string newVariablesJson)
        {
            ValidateInputParameters(existingContainerJson, newVariablesJson);

            var existingContainer = GetContainer(existingContainerJson);

            existingContainer.SetVariables(GetNewVariables(newVariablesJson));

            return JsonConvert.SerializeObject(existingContainer);
        }

        private static Dictionary<string, Variable> GetNewVariables(string newVariables)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, Variable>>(newVariables);
        }

        private static JObject GetContainer(string existingBuildJson)
        {
            return JsonConvert.DeserializeObject<JObject>(existingBuildJson);
        }

        private static void ValidateInputParameters(string existingContainerJson, string newVariablesJson)
        {
            if (string.IsNullOrEmpty(existingContainerJson))
            {
                throw new ArgumentNullException(nameof(existingContainerJson));
            }

            if (string.IsNullOrEmpty(newVariablesJson))
            {
                throw new ArgumentNullException(nameof(newVariablesJson));
            }
        }
    }
}
