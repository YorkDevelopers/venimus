using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserEventsController : BaseUserController
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.UserStore _userStore;

        public UserEventsController(Mongo.EventStore eventStore, Mongo.UserStore userStore)
        {
            _eventStore = eventStore;
            _userStore = userStore;
        }

        /// <summary>
        ///     Allows the current user to retrieve all the future events they have signed up to.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/user/events
        ///
        /// </remarks>
        /// <returns>Array of the ViewAllMyEventRegistrations view model</returns>
        /// <response code="200">Success</response>
        [Authorize]
        [Route("api/user/events")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ViewAllMyEventRegistrations[]>> Get()
        {
            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _userStore.GetUserByID(uniqueID);

            return await _eventStore.GetMyEventRegistrations(existingUser.Id);
        }
    }
}