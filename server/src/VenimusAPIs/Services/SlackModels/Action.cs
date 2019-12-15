using Newtonsoft.Json;

namespace VenimusAPIs.Services.SlackModels
{
    public class Action
    {
        [JsonProperty("action_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ActionID { get; set; } = default!;

        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Value { get; set; } = default!;
    }
}
