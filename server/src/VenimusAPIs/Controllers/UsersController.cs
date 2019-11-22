using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VenimusAPIs.Validation;

namespace VenimusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Mongo.UserStore _userStore;

        public UsersController(Mongo.UserStore userStore)
        {
            _userStore = userStore;
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

            var user = await _userStore.GetUserById(userID);
            if (user == null)
            {
                return NotFound();
            }

            return File(user.ProfilePicture, "image/jpeg");
        }
    }
}