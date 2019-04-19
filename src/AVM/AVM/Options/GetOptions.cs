using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace AVM.Options
{
    [Verb("get", HelpText = "Get various types of data from Azure DevOps.")]
    public class GetOptions
    {
        [Value(2, MetaName = "Type", Required = true, HelpText = "Values: Build, Release, BuildVariables, ReleaseVariables")]
        public AvmObjectType Type { get; set; }
        [Value(3, MetaName = "Id")]
        public string Id { get; set; }
    }
}
