﻿using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace AVM.Options
{
    [Verb("list")]
    public class ListOptions
    {
        [Value(2, HelpText = "Type of object to list: Build, Release")]
        public AvmObjectType Type { get; set; }
    }
}
