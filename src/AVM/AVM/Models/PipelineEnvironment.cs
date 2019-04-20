using System.Collections.Generic;
using AVM.Commands;
using Newtonsoft.Json;

namespace AVM.Models
{
    public class PipelineEnvironment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("variables")]
        public Dictionary<string, Variable> Variables { get; set; }
    }
}