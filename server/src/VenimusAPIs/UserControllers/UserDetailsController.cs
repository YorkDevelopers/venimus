using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserDetailsController : BaseUserController
    {
        private readonly Mongo.UserStore _userStore;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;

        public UserDetailsController(Mongo.UserStore userStore, IStringLocalizer<ResourceMessages> stringLocalizer)
        {
            _userStore = userStore;
            _stringLocalizer = stringLocalizer;
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
        [Route("api/user", Name = "CurrentUserDetails")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ViewMyDetails>> Get()
        {
            var uniqueID = UniqueIDForCurrentUser;

            var user = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            return new ViewMyDetails
            {
                EmailAddress = user.EmailAddress,
                Pronoun = user.Pronoun,
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                Fullname = user.Fullname,
                IsRegistered = user.IsRegistered,
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
        public async Task<ActionResult<ViewMyDetails>> Put([FromServices] IBus bus, [FromBody] UpdateMyDetails updateMyDetails)
        {
            var uniqueID = UniqueIDForCurrentUser;

            var user = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            if (!user.DisplayName.Equals(updateMyDetails.DisplayName, StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateUser = await _userStore.GetUserByDisplayName(updateMyDetails.DisplayName).ConfigureAwait(false);
                if (duplicateUser != null)
                {
                    var message = _stringLocalizer.GetString(Resources.ResourceMessages.USER_ALREADY_EXISTS_WITH_THIS_NAME).Value;
                    ModelState.AddModelError(nameof(updateMyDetails.DisplayName), message);
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
            user.IsRegistered = updateMyDetails.IsRegistered;

            if (!string.IsNullOrWhiteSpace(updateMyDetails.ProfilePictureAsBase64))
            {
                user.ProfilePicture = Convert.FromBase64String(updateMyDetails.ProfilePictureAsBase64);
            }

            await _userStore.UpdateUser(user).ConfigureAwait(false);

            var userChangedMessage = new ServiceBusMessages.UserChangedMessage { UserId = user.Id.ToString() };
            await bus.Publish(userChangedMessage).ConfigureAwait(false);

            return NoContent();
        }
    }
}