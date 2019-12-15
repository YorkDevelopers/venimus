using Newtonsoft.Json;
using System.Collections.Generic;

namespace VenimusAPIs.Services.SlackModels
{
    public class AdvancedMessage
    {
        [JsonProperty("blocks", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<IBlock> Blocks { get; set; } = new List<IBlock>();
    }
}
