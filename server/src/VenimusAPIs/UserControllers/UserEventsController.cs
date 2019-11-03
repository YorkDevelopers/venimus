using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserEventsController : BaseUserController
    {
        private readonly Services.Mongo _mongo;

        public UserEventsController(Services.Mongo mongo)
        {
            _mongo = mongo;
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

            var existingUser = await _mongo.GetUserByID(uniqueID); 
            
            return await _mongo.GetMyEventRegistrations(existingUser.Id);
        }
    }
}