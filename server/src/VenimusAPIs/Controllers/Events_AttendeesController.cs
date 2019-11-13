using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.UserControllers;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class Events_AttendeesController : BaseUserController
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.UserStore _userStore;

        public Events_AttendeesController(Mongo.EventStore eventStore, Mongo.UserStore userStore)
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
        [CallerMustBeGroupMember]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListEventAttendees[]>> Get([FromRoute, Slug]string groupSlug, [FromRoute, Slug]string eventSlug)
        {
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug);
            if (theEvent == null)
            {
                return NotFound();
            }

            var attendees = theEvent.Members.ToArray();

            var users = await _userStore.GetUsersByIds(attendees.Select(m => m.UserId));

            return users.Select(m => new ListEventAttendees
            {
                Bio = m.Bio,
                DisplayName = m.DisplayName,
                EmailAddress = m.EmailAddress,
                Fullname = m.Fullname,
                Pronoun = m.Pronoun,
                Slug = m.Id.ToString(),
                ProfilePictureInBase64 = Convert.ToBase64String(m.ProfilePicture),
                IsHost = attendees.Single(x => x.UserId == m.Id).Host,
                IsSpeaker = attendees.Single(x => x.UserId == m.Id).Speaker,
                IsAttending = attendees.Single(x => x.UserId == m.Id).SignedUp,
                NumberOfGuests = attendees.Single(x => x.UserId == m.Id).NumberOfGuests,
            }).ToArray();
        }
    }
}