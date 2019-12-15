using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace VenimusAPIs.PublicControllers
{
    [ApiController]
    public class SlackWebHookController : ControllerBase
    {
        [Route("public/SlackWebHook")]
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Post([FromServices] Mongo.UserStore userStore)
        {
            var data = Request.Form["payload"];
            var interaction = JsonConvert.DeserializeObject<Interaction>(data);
            var action = interaction.Actions[0];

            if (action.ActionID == "APPROVE_MEMBERSHIP")
            {
                var userID = new ObjectId(action.Value);
                var user = await userStore.GetUserById(userID).ConfigureAwait(false);
                user.IsApproved = true;
                await userStore.UpdateUser(user).ConfigureAwait(false);
            }

            return Ok();
        }

#pragma warning disable CA1034 // Nested types should not be visible
        public class Interaction
        {
            [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Type { get; set; } = default!;

            [JsonProperty("actions", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public Action[] Actions { get; set; } = Array.Empty<Action>();
        }

        public class Action
        {
            [JsonProperty("action_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string ActionID { get; set; } = default!;

            [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Value { get; set; } = default!;
        }
    }
}
