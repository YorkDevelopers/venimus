using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [Route("api/User/Groups")]
    [ApiController]
    public class UserGroupsController : BaseUserController
    {
        private readonly Services.Mongo _mongo;

        private readonly IMapper _mapper;

        public UserGroupsController(Services.Mongo mongo, IMapper mapper)
        {
            _mongo = mongo;
            _mapper = mapper;
        }

        /// <summary>
        ///     Allows the current user to see the groups they are a member of
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/user/groups
        ///
        /// </remarks>
        /// <returns>An array of ListMyGroups view models</returns>
        /// <response code="200">Success</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListMyGroups[]>> Get()
        {
            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _mongo.GetUserByID(uniqueID);

            var groups = await _mongo.RetrieveMyActiveGroups(existingUser.Id);

            var viewModels = _mapper.Map<ListMyGroups[]>(groups);

            return viewModels;
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
            var model = await _mongo.RetrieveGroup(group.GroupSlug);
            if (model == null)
            {
                return NotFound();
            }

            if (model.Members == null)
            {
                model.Members = new List<MongoDB.Bson.ObjectId>();
            }

            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _mongo.GetUserByID(uniqueID);

            model.Members.Add(existingUser.Id);

            await _mongo.UpdateGroup(model);

            return NoContent();
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
        public async Task<IActionResult> DELETE([FromRoute] string groupSlug)
        {
            var model = await _mongo.RetrieveGroup(groupSlug);
            if (model == null)
            {
                return NotFound();
            }

            if (model.Members != null)
            {
                var uniqueID = UniqueIDForCurrentUser;

                var existingUser = await _mongo.GetUserByID(uniqueID);

                model.Members.RemoveAll(m => m == existingUser.Id);

                await _mongo.UpdateGroup(model);
            }

            return NoContent();
        }
    }
}