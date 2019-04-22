using System;
using System.Collections.Generic;
using AVM.Extensions;
using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Json
{
    public class AzureBuild
    {
        public string UpdateBuild(string existingBuildJson, string newVariables)
        {
            ValidateInputParameters(existingBuildJson, newVariables);

            var existingBuild = GetBuild(existingBuildJson);

            existingBuild.SetVariables(GetNewBuildVariables(newVariables));

            return JsonConvert.SerializeObject(existingBuild);
        }

        private static Dictionary<string, Variable> GetNewBuildVariables(string newVariables)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, Variable>>(newVariables);
        }

        private static JObject GetBuild(string existingBuildJson)
        {
            var existingBuild = JsonConvert.DeserializeObject<JObject>(existingBuildJson);
            return existingBuild;
        }

        private static void ValidateInputParameters(string existingBuildJson, string newVariables)
        {
            if (string.IsNullOrEmpty(existingBuildJson))
            {
                throw new ArgumentNullException(nameof(existingBuildJson));
            }

            if (string.IsNullOrEmpty(newVariables))
            {
                throw new ArgumentNullException(nameof(newVariables));
            }
        }
    }
}
