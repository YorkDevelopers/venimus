using Newtonsoft.Json;

namespace VenimusAPIs.Services.SlackModels
{
    public class ButtonText
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; } = default!;

        [JsonProperty("text", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Text { get; set; } = default!;

        [JsonProperty("emoji", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Emoji { get; set; }
    }
}
