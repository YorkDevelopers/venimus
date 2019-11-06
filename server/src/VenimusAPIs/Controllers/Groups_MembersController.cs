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
    public class Groups_MembersController : BaseUserController
    {
        private readonly Services.Mongo _mongo;

        public Groups_MembersController(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        /// <summary>
        ///     Allows you to retrieve the members of a group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo/Members
        ///
        /// </remarks>
        /// <returns>The ListGroupMembers view model</returns>
        /// <response code="200">Success</response>
        /// <response code="401">No Access.</response>
        /// <response code="403">No Permission.</response>
        /// <response code="404">Group does not exist.</response>
        [Authorize]
        [Route("api/Groups/{groupSlug}/Members")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListGroupMembers[]>> Get([FromRoute, Slug]string groupSlug)
        {
            var group = await _mongo.RetrieveGroupBySlug(groupSlug);
            if (group == null)
            {
                return NotFound(); 
            }

            if (group.Members == null)
            {
                group.Members = new List<Group.GroupMember>();
            }

            var memberIds = group.Members.ToArray();

            if (!User.IsInRole("SystemAdministrator"))
            {
                var uniqueID = UniqueIDForCurrentUser;
                var user = await _mongo.GetUserByID(uniqueID);
                if (!memberIds.Any(u => u.Id == user.Id))
                {
                    return Forbid();
                }
            }

            var members = await _mongo.GetUsersByIds(memberIds.Select(m => m.Id));

            return members.Select(m => new ListGroupMembers
            {
                Bio = m.Bio,
                DisplayName = m.DisplayName,
                EmailAddress = m.EmailAddress,
                Fullname = m.Fullname,
                Pronoun = m.Pronoun,
                Slug = m.Id.ToString(),
                ProfilePictureInBase64 = Convert.ToBase64String(m.ProfilePicture),
            }).ToArray();
        }
    }
}