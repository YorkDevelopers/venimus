using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.Models;
using VenimusAPIs.UserControllers;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class Events_AttendeesController : BaseUserController
    {
        private readonly Services.Mongo _mongo;

        public Events_AttendeesController(Services.Mongo mongo)
        {
            _mongo = mongo;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListEventAttendees[]>> Get([FromRoute, Slug]string groupSlug, [FromRoute, Slug]string eventSlug)
        {
            var theEvent = await _mongo.GetEvent(groupSlug, eventSlug);
            if (theEvent == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("SystemAdministrator"))
            {
                var groupMembers = await GetGroupMembers(groupSlug);

                var uniqueID = UniqueIDForCurrentUser;
                var user = await _mongo.GetUserByID(uniqueID);
                if (!groupMembers.Any(u => u.Id == user.Id))
                {
                    return Forbid();
                }
            }

            var attendees = theEvent.Members.ToArray();

            var users = await _mongo.GetUsersByIds(attendees.Select(m => m.UserId));

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
            }).ToArray();
        }

        private async Task<Group.GroupMember[]> GetGroupMembers(string groupSlug)
        {
            var group = await _mongo.RetrieveGroupBySlug(groupSlug);

            if (group.Members == null)
            {
                group.Members = new List<Group.GroupMember>();
            }

            var members = group.Members.ToArray();
            return members;
        }
    }
}