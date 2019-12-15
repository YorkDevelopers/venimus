using Newtonsoft.Json;
using System;

namespace VenimusAPIs.Services.SlackModels
{
    public class ActionBlock : IBlock
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; } = default!;
        
        [JsonProperty("elements", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Element[] Elements { get; set; } = Array.Empty<Element>();
    }
}
