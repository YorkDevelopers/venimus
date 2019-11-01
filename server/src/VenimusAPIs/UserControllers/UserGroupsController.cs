using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public UserGroupsController(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        /// <summary>
        ///     Allows the current user to join a group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/groups
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
 
            var uniqueID = UniqueID;

            var existingUser = await _mongo.GetUserByID(uniqueID);

            model.Members.Add(existingUser.Id);

            await _mongo.UpdateGroup(model);

            return NoContent();
        }
    }
}