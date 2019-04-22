using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AVM.Tests
{
    public static class TestUtilities
    {
        internal static JObject CreateValidReleaseEnvironment()
        {
            return new JObject
            {
                ["id"] = 1,
                ["name"] = "Env 1",
                ["variables"] = new JObject()
            };
        }

        internal static EnvironmentVariables CreateValidEnvironmentVariables()
        {
            return new EnvironmentVariables
            {
                Organization = "Organization",
                Project = "Project",
                Token = "Token"
            };
        }
    }
}
