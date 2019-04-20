using Newtonsoft.Json;

namespace AVM.Models
{
    public class Variable
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("allowOverride")]
        public bool? AllowOverride { get; set; }
        [JsonProperty("isSecret")]
        public bool? IsSecret { get; set; }
    }
}