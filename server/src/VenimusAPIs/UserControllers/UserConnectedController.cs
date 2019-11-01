using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserConnectedController : BaseUserController
    {
        private readonly Services.Mongo _mongo;

        private readonly Services.Auth0API _auth0API;

        public UserConnectedController(Services.Mongo mongo, Services.Auth0API auth0API)
        {
            _mongo = mongo;
            _auth0API = auth0API;
        }

        /// <summary>
        ///     Called once the user has connected to the frontend.  This ensures that they exist in the database.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/connected
        ///     {
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the user</returns>
        /// <response code="201">Success</response>
        [Route("api/Users/Connected")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post()
        {
            var uniqueID = UniqueID;

            var existingUser = await _mongo.GetUserByID(uniqueID);
            if (existingUser == null)
            {
                await CreateOrMergeUser(uniqueID);
            }

            return NoContent();
        }

        private async Task CreateOrMergeUser(string uniqueID)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            var userInfo = await _auth0API.UserInfo(accessToken);

            var existingUser = await _mongo.GetUserByEmailAddress(userInfo.Email);
            if (existingUser == null)
            {
                await CreateNewUser(uniqueID, userInfo);
            }
            else
            {
                await AddIdentityToExistingUser(uniqueID, existingUser);
            }
        }

        private async Task AddIdentityToExistingUser(string uniqueID, Models.User existingUser)
        {
            existingUser.Identities.Add(uniqueID);

            await _mongo.UpdateUser(existingUser);
        }

        private async Task CreateNewUser(string uniqueID, Services.Auth0Models.UserProfile userInfo)
        {
            var newUser = new Models.User
            {
                Identities = new List<string> { uniqueID },
                EmailAddress = userInfo.Email,
            };

            await _mongo.InsertUser(newUser);
        }
    }
}