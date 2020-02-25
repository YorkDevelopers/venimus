using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Extensions;
using VenimusAPIs.Models;
using VenimusAPIs.Mongo;
using VenimusAPIs.Services;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class GroupsMembersController : BaseController
    {
        private readonly UserStore _userStore;
        private readonly GroupStore _groupStore;
        private readonly URLBuilder _urlBuilder;

        public GroupsMembersController(UserStore userStore, GroupStore groupStore, URLBuilder urlBuilder)
        {
            _userStore = userStore;
            _groupStore = groupStore;
            _urlBuilder = urlBuilder;
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
        public async Task<ActionResult<ListGroupMembers[]>> Get(Models.Group? group)
        {
            if (group is null)
            {
                return NotFound();
            }

            var caller = await _userStore.GetUserByID(UniqueIDForCurrentUser).ConfigureAwait(false);

            if (!UserIsASystemAdministrator && !group.UserIsApprovedGroupMember(caller))
            {
                return Forbid();
            }

            return group!.Members.Select(m => new ListGroupMembers
            {
                Bio = m.Bio,
                DisplayName = m.DisplayName,
                EmailAddress = m.EmailAddress,
                Fullname = m.Fullname,
                Pronoun = m.Pronoun,
                Slug = m.UserId.ToString(),
                IsAdministrator = m.IsAdministrator,
                IsApproved = m.IsUserApproved,
                ProfilePicture = _urlBuilder.BuildUserDetailsProfilePictureURL(m.UserId),
            }).ToArray();
        }

        [Authorize]
        [Route("api/Groups/{groupSlug}/Members")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Post(Models.Group? group, [FromBody] AddGroupMember addGroupMember)
        {
            if (group == null)
            {
                return NotFound();
            }

            var userToAdd = await _userStore.GetUserById(new MongoDB.Bson.ObjectId(addGroupMember.Slug)).ConfigureAwait(false);
            if (userToAdd == null)
            {
                return NotFound();
            }

            var caller = await _userStore.GetUserByID(UniqueIDForCurrentUser).ConfigureAwait(false);
            if (!UserIsASystemAdministrator && !group.UserIsGroupAdministrator(caller))
            {
                return Forbid();
            }

            var member = group.Members.SingleOrDefault(mem => mem.UserId == userToAdd.Id);
            if (member == null)
            {
                member = new GroupMember
                {
                    UserId = userToAdd.Id,
                    Bio = userToAdd.Bio,
                    DisplayName = userToAdd.DisplayName,
                    EmailAddress = userToAdd.EmailAddress,
                    Fullname = userToAdd.Fullname,
                    IsUserApproved = userToAdd.IsApproved,
                    Pronoun = userToAdd.Pronoun,
                };

                group.Members.Add(member);
            }
            
            member.IsAdministrator = addGroupMember.IsAdministrator;

            await _groupStore.UpdateGroup(group).ConfigureAwait(false);

            return Ok();
        }
    }
}