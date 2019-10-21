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

        /// <summary>
        ///     Allows you to create a new event for your group.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/groups/YorkCodeDojo/events
        ///     {
        ///         "title" : "Game of Life - Oct 2019",
        ///         "description" : "Tonight we will work in pairs implementing the **classic Game Of Life**"
        ///         "location" : "Room 12"
        ///         "startTime" : "2019-12-12 18:30"
        ///         "endTime" : "2019-12-12 21:00"
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the created event</returns>
        /// <response code="201">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">The group does not exist.</response>
        [Route("api/groups/{groupName}/events")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromRoute] string groupName, [FromBody] CreateNewEvent newEvent)
        {
            var model = _mapper.Map<Models.Event>(newEvent);

            await _mongo.StoreEvent(model);
            return CreatedAtRoute("Events", new { groupName = groupName, eventID = model._id }, newEvent);
        }

        /// <summary>
        ///     Allows you to amend the details of an existing event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/groups/YorkCodeDojo/events/12345
        ///     {
        ///         "title" : "Game of Life - Oct 2019",
        ///         "description" : "Tonight we will work in pairs implementing the **classic Game Of Life**"
        ///         "location" : "Room 12"
        ///         "startTime" : "2019-12-12 18:30"
        ///         "endTime" : "2019-12-12 21:00"
        ///         "host" : "E Betteridge"
        ///         "speaker" : "J Betteridge"
        ///     }
        ///
        /// </remarks>
        /// <returns>No data</returns>
        /// <response code="200">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">The group does not exist.</response>
        [Route("api/groups/{groupName}/events/{eventID}")]
        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Put([FromRoute] string groupName, [FromRoute] string eventID, [FromBody] UpdateEvent amendedEvent)
        {
            var model = await _mongo.RetrieveEvent(eventID);
            _mapper.Map(amendedEvent, model);

            await _mongo.UpdateEvent(model);
            return Ok();
        }

        /// <summary>
        ///     Allows you to amend the details of an existing event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/groups/YorkCodeDojo/events/12345
        ///     {
        ///         "title" : "Game of Life - Oct 2019",
        ///         "description" : "Tonight we will work in pairs implementing the **classic Game Of Life**"
        ///         "location" : null
        ///         "startTime" : "2019-12-12 18:30"
        ///         "endTime" : "2019-12-12 21:00"
        ///         "host" : "E Betteridge"
        ///         "speaker" : "J Betteridge"
        ///     }
        ///
        /// </remarks>
        /// <returns>No data</returns>
        /// <response code="200">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">The group does not exist.</response>
        [Route("api/groups/{groupName}/events/{eventID}")]
        [Authorize]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Patch([FromRoute] string groupName, [FromRoute] string eventID, [FromBody] PartiallyUpdateEvent amendedEvent)
        {
            var model = await _mongo.RetrieveEvent(eventID);

            model.Description = amendedEvent.Description ?? model.Description;
            model.Title = amendedEvent.Title ?? model.Title;
            model.Location = amendedEvent.Location ?? model.Location;
            model.StartTime = amendedEvent.StartTime ?? model.StartTime;
            model.EndTime = amendedEvent.EndTime ?? model.EndTime;

            await _mongo.UpdateEvent(model);
            return Ok();
        }

        /// <summary>
        ///     Allows you to retrieve the details of an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo/events/1234
        ///
        /// </remarks>
        /// <returns>The GetEvent view model</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Group or Event does not exist.</response>
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