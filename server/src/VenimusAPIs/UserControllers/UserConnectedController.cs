using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.Services;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserConnectedController : BaseUserController
    {
        private readonly Mongo.UserStore _userStore;

        private readonly Auth0API _auth0API;

        private readonly URLBuilder _urlBuilder;

        private readonly IHttpClientFactory _httpClientFactory;

        public UserConnectedController(Mongo.UserStore userStore, Auth0API auth0API, URLBuilder urlBuilder, IHttpClientFactory httpClientFactory)
        {
            _userStore = userStore;
            _auth0API = auth0API;
            _urlBuilder = urlBuilder;
            _httpClientFactory = httpClientFactory;
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
        [Route("api/user/connected")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post()
        {
            var uniqueID = UniqueIDForCurrentUser;

            var theUser = await _userStore.GetUserByID(uniqueID);
            var newUser = theUser == null;

            if (theUser == null)
            {
                (theUser, newUser) = await CreateOrMergeUser(uniqueID);
            }

            Response.Headers.Add("ProfilePictureURL", new Microsoft.Extensions.Primitives.StringValues(_urlBuilder.BuildUserDetailsProfilePictureURL(theUser)));

            if (newUser)
            {
                Response.Headers.Add("NewUser", new Microsoft.Extensions.Primitives.StringValues("true"));
                return CreatedAtRoute("CurrentUserDetails", new { }, null);
            }
            else
            {
                Response.Headers.Add("NewUser", new Microsoft.Extensions.Primitives.StringValues("false"));
                Response.Headers.Add("Location", new Microsoft.Extensions.Primitives.StringValues(_urlBuilder.BuildCurrentUserDetailsURL()));
                return NoContent();
            }
        }

        private async Task<(Models.User theUser, bool isNew)> CreateOrMergeUser(string uniqueID)
        {
            var accessToken = User.FindFirst("access_token")?.Value;
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new System.Exception("No access token!");
            }

            var userInfo = await _auth0API.UserInfo(accessToken);

            var theUser = await _userStore.GetUserByEmailAddress(userInfo.Email);
            if (theUser == null)
            {
                var newUser = await CreateNewUser(uniqueID, userInfo);
                return (newUser, true);
            }
            else
            {
                await AddIdentityToExistingUser(uniqueID, theUser);
                return (theUser, false);
            }
        }

        private async Task AddIdentityToExistingUser(string uniqueID, Models.User existingUser)
        {
            existingUser.Identities.Add(uniqueID);

            await _userStore.UpdateUser(existingUser);
        }

        private async Task<Models.User> CreateNewUser(string uniqueID, Services.Auth0Models.UserProfile userInfo)
        {
            var newUser = new Models.User
            {
                Identities = new List<string> { uniqueID },
                EmailAddress = userInfo.Email,
                Bio = string.Empty,
                DisplayName = string.Empty,
                Fullname = userInfo.Name,
                Pronoun = string.Empty,
                ProfilePicture = await DownloadImage(userInfo.Picture),
            };

            await _userStore.InsertUser(newUser);

            return newUser;
        }

        private async Task<byte[]> DownloadImage(string url)
        {
            var client = _httpClientFactory.CreateClient("ImageSource");
            return await client.GetByteArrayAsync(url);
        }
    }
}