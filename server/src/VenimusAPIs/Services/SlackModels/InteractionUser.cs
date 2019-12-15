using Newtonsoft.Json;

namespace VenimusAPIs.Services.SlackModels
{
    public class InteractionUser
    {
        [JsonProperty("username", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string UserName { get; set; } = default!;
    }
}
