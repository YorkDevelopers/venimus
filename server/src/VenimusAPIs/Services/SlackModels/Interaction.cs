using Newtonsoft.Json;
using System;

namespace VenimusAPIs.Services.SlackModels
{
    public class Interaction
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; } = default!;

        [JsonProperty("actions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Action[] Actions { get; set; } = Array.Empty<Action>();

        [JsonProperty("response_url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri ResponseURL { get; set; } = default!;

        [JsonProperty("user", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public InteractionUser User { get; set; } = default!;
    }
}
