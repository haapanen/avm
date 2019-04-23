using System;
using System.Collections.Generic;
using System.Text;
using AVM.Azure;
using AVM.Models;
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

        internal static JObject CreateValidBuild()
        {
            var build = new JObject
            {
                ["id"] = 1,
                ["name"] = "Build 1",
                ["variables"] = new JObject()
            };
            return build;
        }

        internal static JObject CreateValidRelease()
        {
            var release = new JObject
            {
                ["id"] = 1,
                ["name"] = "Release 1",
                ["variables"] = new JObject(),
                ["environments"] = new JArray()
            };
            return release;
        }

        internal static JObject CreateValidVariableJObject()
        {
            return new JObject
            {
                ["value"] = "value",
                ["isSecret"] = false,
                ["allowOverride"] = false
            };
        }

        internal static Variable CreateValidVariable()
        {
            return new Variable
            {
                Value = "value",
                IsSecret = false,
                AllowOverride = false
            };
        }
    }
}
