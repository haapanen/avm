using System;
using AVM.Extensions;
using AVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Azure
{
    public class ReleaseTransformer
    {
        public string UpdateRelease(string existingReleaseJson, string newReleaseVariablesJson)
        {
            ValidateInputParameters(existingReleaseJson, newReleaseVariablesJson);

            var existingRelease = GetExistingRelease(existingReleaseJson);
            var newReleaseVariables = GetNewReleaseVariables(newReleaseVariablesJson);

            SetReleaseGlobalVariables(existingRelease, newReleaseVariables);

            SetReleaseEnvironmentSpecificVariables(newReleaseVariables, existingRelease);

            return JsonConvert.SerializeObject(existingRelease);
        }

        private static void SetReleaseEnvironmentSpecificVariables(ReleaseVariables newReleaseVariables,
            JToken existingRelease)
        {
            foreach (var env in newReleaseVariables.Environments)
            {
                GetEnvironment(existingRelease, env).SetVariables(env.Variables);
            }
        }

        private static JToken GetEnvironment(JToken existingRelease, PipelineEnvironment env)
        {
            var matchingEnv = existingRelease.SelectToken($"$.environments[?(@.id=={env.Id})]");
            if (matchingEnv == null)
            {
                throw new ArgumentException($"Could not find an environment with id [{env.Id}]");
            }

            return matchingEnv;
        }

        private static void SetReleaseGlobalVariables(JToken existingRelease, ReleaseVariables newReleaseVariables)
        {
            existingRelease.SetVariables(newReleaseVariables.Variables);
        }

        private static ReleaseVariables GetNewReleaseVariables(string newReleaseVariablesJson)
        {
            var newReleaseVariables = JsonConvert.DeserializeObject<ReleaseVariables>(newReleaseVariablesJson);
            return newReleaseVariables;
        }

        private static JObject GetExistingRelease(string existingReleaseJson)
        {
            var existingRelease = JsonConvert.DeserializeObject<JObject>(existingReleaseJson);
            return existingRelease;
        }

        private static void ValidateInputParameters(string existingReleaseJson, string newReleaseVariablesJson)
        {
            if (string.IsNullOrEmpty(existingReleaseJson))
            {
                throw new ArgumentNullException(existingReleaseJson);
            }

            if (string.IsNullOrEmpty(newReleaseVariablesJson))
            {
                throw new ArgumentNullException(newReleaseVariablesJson);
            }
        }
    }
}
