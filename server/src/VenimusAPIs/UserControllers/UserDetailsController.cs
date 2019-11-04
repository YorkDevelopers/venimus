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

        /// <summary>
        ///     Allows the current user to update their personal profile
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/user
        ///     {
        ///         "pronoun" : "Him"
        ///         "Fullname" : "David Betteridge",
        ///         "DisplayName" : "David B",
        ///         "Bio" : "I am me",
        ///         "profilePictureAsBase64" : "123123123",
        ///     }
        ///
        /// </remarks>
        /// <returns>NoContent</returns>
        /// <response code="204">Success</response>
        [Authorize]
        [Route("api/user")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ViewMyDetails>> Put([FromBody] UpdateMyDetails updateMyDetails)
        {
            var uniqueID = UniqueIDForCurrentUser;

            var user = await _mongo.GetUserByID(uniqueID);

            if (!user.DisplayName.Equals(updateMyDetails.DisplayName, StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateUser = await _mongo.GetUserByDisplayName(updateMyDetails.DisplayName);
                if (duplicateUser != null)
                {
                    ModelState.AddModelError(nameof(updateMyDetails.DisplayName), "A user with this display name already exists.");
                }
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            user.Bio = updateMyDetails.Bio;
            user.Pronoun = updateMyDetails.Pronoun;
            user.DisplayName = updateMyDetails.DisplayName;
            user.Fullname = updateMyDetails.Fullname;
            user.ProfilePicture = Convert.FromBase64String(updateMyDetails.ProfilePictureAsBase64);

            await _mongo.UpdateUser(user);

            return NoContent();
        }
    }
}