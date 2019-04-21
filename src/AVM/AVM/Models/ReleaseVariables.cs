using System.Collections.Generic;
using AVM.Commands;
using Newtonsoft.Json;

namespace AVM.Models
{
    public class ReleaseVariables
    {
        [JsonProperty("variables")]
        public Dictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
        [JsonProperty("environments")]
        public List<PipelineEnvironment> Environments { get; set; } = new List<PipelineEnvironment>();
    }
}