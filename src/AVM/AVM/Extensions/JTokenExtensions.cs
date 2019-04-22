using System;
using System.Collections.Generic;
using System.Text;
using AVM.Models;
using Newtonsoft.Json.Linq;

namespace AVM.Extensions
{
    public static class JTokenExtensions
    {
        public static void SetVariables(this JToken token, Dictionary<string, Variable> variables)
        {
            foreach (var kvp in variables)
            {
                token.SelectToken("$.variables").SetVariableValue(kvp);
            }
        }

        public static void SetVariableValue(this JToken variableContainer, KeyValuePair<string, Variable> variable)
        {
            if (variable.Value == null)
            {
                variableContainer[variable.Key] = null;
            }
            else
            {
                variableContainer[variable.Key] = new JObject();
                variableContainer[variable.Key]["value"] = variable.Value.Value;
                variableContainer[variable.Key]["allowOverride"] = variable.Value.AllowOverride;
                variableContainer[variable.Key]["isSecret"] = variable.Value.IsSecret;
            }
        }
    }
}
