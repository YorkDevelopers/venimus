using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.Mongo;
using VenimusAPIs.Services;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class GroupsMembersController : Controller
    {
        private readonly UserStore _userStore;
        private readonly GroupStore _groupStore;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;
        private readonly URLBuilder _urlBuilder;

        public GroupsMembersController(UserStore userStore, GroupStore groupStore, IStringLocalizer<ResourceMessages> stringLocalizer, URLBuilder urlBuilder)
        {
            _userStore = userStore;
            _groupStore = groupStore;
            _stringLocalizer = stringLocalizer;
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
        [CallerMustBeApprovedGroupMember]
        public async Task<ActionResult<ListGroupMembers[]>> Get([FromRoute, Slug]string groupSlug)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);

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
        [CallerMustBeGroupAdministrator]
        [Route("api/Groups/{groupSlug}/Members")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Post([FromRoute, Slug]string groupSlug, [FromBody] AddGroupMember addGroupMember)
        {
            var model = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);
            if (model == null)
            {
                return NotFound();
            }

            var existingUser = await _userStore.GetUserById(new MongoDB.Bson.ObjectId(addGroupMember.Slug)).ConfigureAwait(false);
            if (existingUser == null)
            {
                return NotFound();
            }

            model.Members.Add(new GroupMember
            {
                UserId = existingUser.Id,
                Bio = existingUser.Bio,
                DisplayName = existingUser.DisplayName,
                EmailAddress = existingUser.EmailAddress,
                Fullname = existingUser.Fullname,
                IsAdministrator = addGroupMember.IsAdministrator,
                IsUserApproved = existingUser.IsApproved,
                Pronoun = existingUser.Pronoun,
            });

            await _groupStore.UpdateGroup(model).ConfigureAwait(false);

            return Ok();
        }
    }
}