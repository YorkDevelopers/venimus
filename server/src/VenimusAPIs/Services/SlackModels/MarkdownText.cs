#pragma warning disable CA1822 // Can be marked as static
using Newtonsoft.Json;

namespace VenimusAPIs.Services.SlackModels
{
    public class MarkdownText
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type => "mrkdwn";

        [JsonProperty("text", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Text { get; set; } = default!;
    }
}
