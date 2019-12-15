using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace VenimusAPIs.PublicControllers
{
    [ApiController]
    public class SlackWebHookController : ControllerBase
    {
        [Route("public/SlackWebHook")]
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult Post()
        {
            var data = Request.Form["payload"];
            var interaction = JsonConvert.DeserializeObject<Interaction>(data);

            return Ok(interaction.type);
        }

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public class Interaction
        {
            public string type { get; set; } = default!;
        }
    }
}
