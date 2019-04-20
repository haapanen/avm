using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace AVM.Options
{
    [Verb("set")]
    public class SetOptions
    {
        [Value(2, HelpText = "Type of object: Build, Release, BuildVariables, ReleaseVariables")]
        public AvmObjectType Type { get; set; }
        [Value(3, HelpText = "Object ID")]
        public string Id { get; set; }
        [Value(4, HelpText = "Source path")]
        public string SourceFilePath { get; set; }
    }
}
