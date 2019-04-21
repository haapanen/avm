using System;
using System.Collections.Generic;
using System.Text;
using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Json
{
    public class AzureBuild : AzureVariableContainer
    {
        public string UpdateBuild(string existingBuildJson, string newVariables)
        {

            if (string.IsNullOrEmpty(existingBuildJson))
            {
                throw new ArgumentNullException(nameof(existingBuildJson));
            }

            if (string.IsNullOrEmpty(newVariables))
            {
                throw new ArgumentNullException(nameof(newVariables));
            }

            var existingBuild = JsonConvert.DeserializeObject<JObject>(existingBuildJson);
            var variables = JsonConvert.DeserializeObject<Dictionary<string, Variable>>(newVariables);

            SetValues(existingBuild, variables);
            return JsonConvert.SerializeObject(existingBuild);
        }
    }
}
