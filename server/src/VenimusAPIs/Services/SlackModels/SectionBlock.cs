using Newtonsoft.Json;

namespace VenimusAPIs.Services.SlackModels
{
    public class SectionBlock : IBlock
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; } = default!;
        
        [JsonProperty("text", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MarkdownText Text { get; set; } = default!;

        [JsonProperty("accessory", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Accessory? Accessory { get; set; }
    }
}
