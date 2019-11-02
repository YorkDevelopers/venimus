using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserGroupsEventsController : BaseUserController
    {
        private readonly Services.Mongo _mongo;

        private readonly IMapper _mapper;

        public UserGroupsEventsController(Services.Mongo mongo, IMapper mapper)
        {
            _mongo = mongo;
            _mapper = mapper;
        }

        /// <summary>
        ///     Allows the current user to sign up to an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/user/groups/YorkCodeDojo/Events
        ///     {
        ///         "eventSlug" : "Nov2019",
        ///         "numberOfGuest" : 1,
        ///         "dietaryRequirements" : "Milk free",
        ///         "messageToOrganiser" : "I might be 10 minutes late"
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the created group</returns>
        /// <response code="204">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">Group does not exist</response>
        [Authorize]
        [HttpPost]
        [Route("api/User/Groups/{groupSlug}/Events")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromRoute, Slug] string groupSlug, [FromBody] SignUpToEvent signUpDetails)
        {
            var eventSlug = signUpDetails.EventSlug;
            var theEvent = await _mongo.GetEvent(groupSlug, eventSlug);

            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _mongo.GetUserByID(uniqueID);

            if (theEvent.Members == null)
            {
                theEvent.Members = new List<Models.Event.EventAttendees>();
            }

            theEvent.Members.Add(new Models.Event.EventAttendees
            {
                DietaryRequirements = signUpDetails.DietaryRequirements,
                MessageToOrganiser = signUpDetails.MessageToOrganiser,
                NumberOfGuests = signUpDetails.NumberOfGuests,
                UserId = existingUser.Id,
                SignedUp = true,
            });

            await _mongo.UpdateEvent(theEvent);

            return CreatedAtRoute("EventRegistration", new { groupSlug, eventSlug }, null);
        }

        /// <summary>
        ///     Allows the current user to retrieve their registration details for an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/user/groups/YorkCodeDojo/Events/Nov2019
        ///
        /// </remarks>
        /// <returns>The GetEvent view model</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Group or Event does not exist.</response>
        [Authorize]
        [Route("api/user/groups/{groupSlug}/events/{eventSlug}", Name = "EventRegistration")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetEvent> Get([FromRoute] string groupSlug, [FromRoute] string eventSlug)
        {
            return NotFound();
        }
    }
}