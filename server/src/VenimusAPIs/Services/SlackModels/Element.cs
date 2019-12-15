using Newtonsoft.Json;

namespace VenimusAPIs.Services.SlackModels
{
    public class Element
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; } = default!;

        [JsonProperty("action_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ActionID { get; set; } = default!;

        [JsonProperty("style", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Style { get; set; } = default!;

        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Value { get; set; } = default!;

        [JsonProperty("text", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ButtonText Text { get; set; } = new ButtonText();
    }
}
