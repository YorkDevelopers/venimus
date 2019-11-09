using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VenimusAPIs.UserControllers;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class EventsController : BaseUserController
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
        ///         "slug" : "Oct2019"
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
        [Route("api/groups/{groupSlug}/events")]
        [Authorize]
        [CallerMustBeGroupAdministrator]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromRoute, Slug] string groupSlug, [FromBody] CreateEvent newEvent)
        {
            if (newEvent.StartTimeUTC < DateTime.UtcNow)
            {
                ModelState.AddModelError(nameof(newEvent.StartTimeUTC), "You cannot create an event in the past.");
            }

            if (newEvent.StartTimeUTC >= newEvent.EndTimeUTC)
            {
                ModelState.AddModelError(nameof(newEvent.EndTimeUTC), "You cannot create an event which ends before it starts.");
            }

            var duplicate = await _mongo.GetEvent(groupSlug, newEvent.Slug);
            if (duplicate != null)
            {
                ModelState.AddModelError(nameof(newEvent.Slug), "An event with this slug already exists for this group.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var group = await _mongo.RetrieveGroupBySlug(groupSlug);

            var model = _mapper.Map<Models.Event>(newEvent);
            model.GroupSlug = groupSlug;
            model.GroupId = group.Id;
            model.GroupName = group.Name;

            await _mongo.StoreEvent(model);
            return CreatedAtRoute("Events", new { groupSlug = groupSlug, eventSlug = model.Slug }, newEvent);
        }

        /// <summary>
        ///     Allows you to amend the details of an existing event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/groups/YorkCodeDojo/events/12345
        ///     {
        ///         "slug" : "Oct2019"
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
        /// <response code="204">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">The group or event does not exist.</response>
        [Route("api/groups/{groupSlug}/events/{eventSlug}")]
        [Authorize]
        [CallerMustBeGroupAdministrator]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put([FromRoute, Slug] string groupSlug, [FromRoute, Slug] string eventSlug, [FromBody] UpdateEvent amendedEvent)
        {
            var model = await _mongo.GetEvent(groupSlug, eventSlug);
            if (model == null)
            {
                return NotFound();
            }

            if (!model.Slug.Equals(amendedEvent.Slug, StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicate = await _mongo.GetEvent(groupSlug, amendedEvent.Slug);
                if (duplicate != null)
                {
                    ModelState.AddModelError(nameof(amendedEvent.Slug), "An event with this slug already exists for this group.");
                }
            }

            if (amendedEvent.StartTimeUTC >= amendedEvent.EndTimeUTC)
            {
                ModelState.AddModelError(nameof(amendedEvent.EndTimeUTC), "You cannot create an event which ends before it starts.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(amendedEvent, model);

            await _mongo.UpdateEvent(model);
            return NoContent();
        }

        /// <summary>
        ///     Allows an administrator to delete an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/groups/YorkCodeDojo/events/1234
        ///
        /// </remarks>
        /// <returns>NoContent</returns>
        /// <response code="204">Success</response>
        [Authorize]
        [Route("api/groups/{groupSlug}/events/{eventSlug}")]
        [HttpDelete]
        [CallerMustBeGroupAdministrator]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromRoute] string groupSlug, [FromRoute] string eventSlug)
        {
            var model = await _mongo.GetEvent(groupSlug, eventSlug);
            if (model != null)
            {
                if (model.EndTimeUTC < DateTime.UtcNow)
                {
                    var details = new ValidationProblemDetails { Detail = "The event cannot be deleted as it has already taken place." };
                    return ValidationProblem(details);
                }

                await _mongo.DeleteEvent(model);
            }

            return NoContent();
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
        [CallerMustBeGroupMember]
        [Route("api/groups/{groupSlug}/events/{eventSlug}", Name = "Events")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetEvent>> Get([FromRoute, Slug] string groupSlug, [FromRoute, Slug] string eventSlug)
        {
            var model = await _mongo.GetEvent(groupSlug, eventSlug);
            if (model == null)
            {
                return NotFound();
            }

            return new GetEvent
            {
                EventDescription = model.Description,
                EventFinishesUTC = model.EndTimeUTC,
                EventSlug = eventSlug,
                EventStartsUTC = model.StartTimeUTC,
                EventTitle = model.Title,
                GroupName = model.GroupName,
                EventLocation = model.Location,
            };
        }
    }
}