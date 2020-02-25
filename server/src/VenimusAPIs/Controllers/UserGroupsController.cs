using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [Route("api/User/Groups")]
    [ApiController]
    public class UserGroupsController : BaseController
    {
        private readonly Mongo.GroupStore _groupStore;
        private readonly Mongo.UserStore _userStore;

        public UserGroupsController(Mongo.GroupStore groupStore, Mongo.UserStore userStore)
        {
            _groupStore = groupStore;
            _userStore = userStore;
        }

        /// <summary>
        ///     Allows the current user to join a group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/user/groups
        ///     {
        ///         "groupSlug" : "YorkCodeDojo",
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the created group</returns>
        /// <response code="204">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">Group does not exist</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromBody] JoinGroup group)
        {
            var model = await _groupStore.RetrieveGroupBySlug(group.GroupSlug).ConfigureAwait(false);
            if (model == null)
            {
                return NotFound();
            }

            var uniqueID = UniqueIDForCurrentUser;

            var caller = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            if (!model.Members.Any(m => m.UserId == caller.Id))
            {
                model.Members.Add(new GroupMember
                {
                    UserId = caller.Id,
                    Bio = caller.Bio,
                    DisplayName = caller.DisplayName,
                    EmailAddress = caller.EmailAddress,
                    Fullname = caller.Fullname,
                    IsAdministrator = false,
                    Pronoun = caller.Pronoun,
                });

                await _groupStore.UpdateGroup(model).ConfigureAwait(false);
            }

            return CreatedAtRoute("Groups", new { groupSlug = group.GroupSlug }, null);
        }

        /// <summary>
        ///     Allows the current user to leave a group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/user/groups/YorkCodeDojo
        ///
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="204">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">Group does not exist.</response>
        [Authorize]
        [Route("{groupSlug}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DELETE(Models.Group? group)
        {
            if (group is null)
            {
                return NotFound();
            }

            var uniqueID = UniqueIDForCurrentUser;
            var caller = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            if (group.Members == null || !group.Members.Any(m => m.UserId == caller.Id))
            {
                return NoContent();
            }

            group!.Members.RemoveAll(m => m.UserId == caller.Id);
            await _groupStore.UpdateGroup(group).ConfigureAwait(false);

            return NoContent();
        }
    }
}