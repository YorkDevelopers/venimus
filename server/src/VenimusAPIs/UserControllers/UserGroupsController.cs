﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [Route("api/User/Groups")]
    [ApiController]
    public class UserGroupsController : BaseUserController
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

            var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            if (!model.Members.Any(m => m.UserId == existingUser.Id))
            {
                model.Members.Add(new GroupMember
                {
                    UserId = existingUser.Id,
                    Bio = existingUser.Bio,
                    DisplayName = existingUser.DisplayName,
                    EmailAddress = existingUser.EmailAddress,
                    Fullname = existingUser.Fullname,
                    IsAdministrator = false,
                    Pronoun = existingUser.Pronoun,
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
        [CallerMustBeGroupMember(CanBeSystemAdministratorInstead = false, UseNoContentRatherThanForbidden = true)]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DELETE([FromRoute, Slug] string groupSlug)
        {
            var model = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);

            var uniqueID = UniqueIDForCurrentUser;
            var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            model!.Members.RemoveAll(m => m.UserId == existingUser.Id);
            await _groupStore.UpdateGroup(model).ConfigureAwait(false);

            return NoContent();
        }
    }
}