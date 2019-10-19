using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly Services.Mongo _mongo;

        private readonly IMapper _mapper;

        public EventsController(Services.Mongo mongo, IMapper mapper)
        {
            _mongo = mongo;
            _mapper = mapper;
        }

        [Route("api/groups/{groupName}/events")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromRoute] string groupName, [FromBody] CreateNewEvent newEvent)
        {
            var model = _mapper.Map<Models.Event>(newEvent);
            model.EventID = Guid.NewGuid().ToString();

            await _mongo.StoreEvent(model);
            return CreatedAtRoute("Events", new { groupName = groupName,  eventID = model.EventID }, newEvent);
        }

        [Authorize]
        [Route("api/groups/{groupName}/events/{eventID}", Name = "Events")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetEvent> Get([FromRoute] string groupName, [FromRoute] string eventID)
        {
            return NotFound();
        }
    }
}