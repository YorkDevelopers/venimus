using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VenimusAPIs.Services;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Mongo.UserStore _userStore;
        private readonly URLBuilder _urlBuilder;

        public UsersController(Mongo.UserStore userStore, URLBuilder urlBuilder)
        {
            _userStore = userStore;
            _urlBuilder = urlBuilder;
        }

        /// <summary>
        ///     Allows you to get the details for a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users?displayName=davidb
        ///
        /// </remarks>
        /// <returns>GetUser object</returns>
        /// <response code="200">Success</response>
        [Route("/api/users")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [CallerMustBeApprovedUser]
        public async Task<ActionResult<GetUser>> GetUserDetails([FromQuery] string displayName)
        {
            var user = await _userStore.GetUserByDisplayName(displayName).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            return new GetUser
            {
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                Fullname = user.Fullname,
                Pronoun = user.Pronoun,
                Slug = user.Id.ToString(),
                ProfileURL = _urlBuilder.BuildUserDetailsProfilePictureURL(user),
            };
        }

        /// <summary>
        ///     Allows you to get the profile picture for a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/YORKCODEDOJO/profilepicture
        ///
        /// </remarks>
        /// <returns>byte array for the image</returns>
        /// <response code="200">Success</response>
        [Route("/api/users/{userSlug}/profilepicture")]
        [HttpGet]
        [ResponseCache(Duration = 300)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLogo([FromRoute, Slug] string userSlug)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(userSlug, out var userID))
            {
                return NotFound();
            }

            var user = await _userStore.GetUserById(userID).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            return File(user.ProfilePicture, "image/jpeg");
        }
    }
}