using Newtonsoft.Json;
using System;

namespace VenimusAPIs.Services.SlackModels
{
    public class Accessory
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; } = default!;

        [JsonProperty("image_url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri ImageURL { get; set; } = default!;

        [JsonProperty("alt_text", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AltText { get; set; } = default!;
    }
}
