using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserDetailsController : BaseUserController
    {
        private readonly Services.Mongo _mongo;

        public UserDetailsController(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        /// <summary>
        ///     Allows the current user to retrieve their personal profile
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/user
        ///
        /// </remarks>
        /// <returns>ViewMyDetails view model</returns>
        /// <response code="200">Success</response>
        [Authorize]
        [Route("api/user")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ViewMyDetails>> Get()
        {
            var uniqueID = UniqueIDForCurrentUser;

            var user = await _mongo.GetUserByID(uniqueID);

            return new ViewMyDetails
            {
                EmailAddress = user.EmailAddress,
                Pronoun = user.Pronoun,
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                Fullname = user.Fullname,
                ProfilePictureAsBase64 = Convert.ToBase64String(user.ProfilePicture),
            };
        }
    }
}