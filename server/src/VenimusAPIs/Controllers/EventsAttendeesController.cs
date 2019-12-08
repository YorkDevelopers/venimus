using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.UserControllers;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class EventsAttendeesController : BaseUserController
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.UserStore _userStore;

        public EventsAttendeesController(Mongo.EventStore eventStore, Mongo.UserStore userStore)
        {
            _eventStore = eventStore;
            _userStore = userStore;
        }

        /// <summary>
        ///     Allows you to retrieve the members of an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo/Events/Nov2019/Members
        ///
        /// </remarks>
        /// <returns>The ListEventAttendees view model</returns>
        /// <response code="200">Success</response>
        /// <response code="401">No Access.</response>
        /// <response code="403">No Permission.</response>
        /// <response code="404">Group does not exist.</response>
        [Authorize]
        [Route("api/Groups/{groupSlug}/Events/{eventSlug}/Members")]
        [HttpGet]
        [CallerMustBeApprovedGroupMember]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListEventAttendees[]>> Get([FromRoute, Slug]string groupSlug, [FromRoute, Slug]string eventSlug)
        {
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
            if (theEvent == null)
            {
                return NotFound();
            }

            return theEvent.Members.Select(attendee => new ListEventAttendees
            {
                Bio = attendee.Bio,
                DisplayName = attendee.DisplayName,
                EmailAddress = attendee.EmailAddress,
                Fullname = attendee.Fullname,
                Pronoun = attendee.Pronoun,
                Slug = attendee.UserId.ToString(),
                IsHost = attendee.Host,
                IsSpeaker = attendee.Speaker,
                IsAttending = attendee.SignedUp,
                NumberOfGuests = attendee.NumberOfGuests,
            }).ToArray();
        }
    }
}