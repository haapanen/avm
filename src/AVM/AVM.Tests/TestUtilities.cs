using System;
using System.Collections.Generic;
using System.Text;
using AVM.Azure;
using Newtonsoft.Json.Linq;
using NSubstitute;

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

        internal static IUrlStore CreateValidUrlStore()
        {
            return Substitute.For<IUrlStore>();
        }
    }
}
