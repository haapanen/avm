using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AVM.Json
{
    public class AzureRelease : AzureVariableContainer
    {
        public string UpdateRelease(string existingReleaseJson, string newReleaseVariablesJson)
        {
            if (string.IsNullOrEmpty(existingReleaseJson))
            {
                throw new ArgumentNullException(existingReleaseJson);
            }

            if (string.IsNullOrEmpty(newReleaseVariablesJson))
            {
                throw new ArgumentNullException(newReleaseVariablesJson);
            }

            var existingRelease = JsonConvert.DeserializeObject<JObject>(existingReleaseJson);
            var newReleaseVariables = JsonConvert.DeserializeObject<ReleaseVariables>(newReleaseVariablesJson);

            SetValues(existingRelease, newReleaseVariables.Variables);

            foreach (var env in newReleaseVariables.Environments)
            {
                var matchingEnv = existingRelease.SelectToken($"$.environments[?(@.id=={env.Id})]");
                if (matchingEnv == null)
                {
                    throw new ArgumentException($"Could not find an environment with id [{env.Id}]");
                }

                SetValues(matchingEnv, env.Variables);
            }

            return JsonConvert.SerializeObject(existingRelease);
        }
    }
}
