using System;
using System.Collections.Generic;
using System.Text;
using AVM.Models;
using Newtonsoft.Json.Linq;

namespace AVM.Json
{
    public abstract class AzureVariableContainer
    {
        protected void SetValues(JToken token, Dictionary<string, Variable> variables)
        {
            foreach (var kvp in variables)
            {
                var variablesToken = token.SelectToken("$.variables");

                SetValue(kvp, variablesToken);
            }
        }

        protected static void SetValue(KeyValuePair<string, Variable> kvp, JToken variablesToken)
        {
            if (kvp.Value == null)
            {
                variablesToken[kvp.Key] = null;
            }
            else
            {
                variablesToken[kvp.Key] = new JObject();
                variablesToken[kvp.Key]["value"] = kvp.Value.Value;
                variablesToken[kvp.Key]["allowOverride"] = kvp.Value.AllowOverride;
                variablesToken[kvp.Key]["isSecret"] = kvp.Value.IsSecret;
            }
        }
    }
}
