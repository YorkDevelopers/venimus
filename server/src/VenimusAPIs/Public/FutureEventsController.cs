using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class FutureEventsController : ControllerBase
    {
        private readonly Services.Mongo _mongo;

        public FutureEventsController(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        /// <summary>
        ///     Allows you request the list of future events.  Maximum of 10 per group.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /public/FutureEvents
        ///
        /// </remarks>
        /// <returns>Array of FutureEvent view models</returns>
        /// <response code="200">Success</response>
        [Route("public/FutureEvents")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<FutureEvent>>> Get()
        {
            var futureEvents = await _mongo.GetFutureEvents();

            return futureEvents;
        }
    }
}