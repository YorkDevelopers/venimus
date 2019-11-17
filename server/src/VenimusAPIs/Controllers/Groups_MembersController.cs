﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class Groups_MembersController : Controller
    {
        private readonly Mongo.GroupStore _groupStore;
        private readonly IStringLocalizer<Messages> _stringLocalizer;

        public Groups_MembersController(Mongo.GroupStore groupStore, IStringLocalizer<Messages> stringLocalizer)
        {
            _groupStore = groupStore;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        ///     Allows you to retrieve the members of a group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo/Members
        ///
        /// </remarks>
        /// <returns>The ListGroupMembers view model</returns>
        /// <response code="200">Success</response>
        /// <response code="401">No Access.</response>
        /// <response code="403">No Permission.</response>
        /// <response code="404">Group does not exist.</response>
        [Authorize]
        [Route("api/Groups/{groupSlug}/Members")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [CallerMustBeApprovedGroupMember]
        public async Task<ActionResult<ListGroupMembers[]>> Get([FromRoute, Slug]string groupSlug)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug);

            return group.Members.Select(m => new ListGroupMembers
            {
                Bio = m.Bio,
                DisplayName = m.DisplayName,
                EmailAddress = m.EmailAddress,
                Fullname = m.Fullname,
                Pronoun = m.Pronoun,
                Slug = m.UserId.ToString(),
                IsAdministrator = m.IsAdministrator,
                IsApproved = m.IsApproved,
            }).ToArray();
        }

        /// <summary>
        ///     Allows a group administrator to approve a new users membership
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/groups/YorkCodeDojo/ApprovedMembers
        ///     {
        ///         userSlug : 123455
        ///     }
        ///
        /// </remarks>
        /// <returns>OK</returns>
        /// <response code="200">Success</response>
        /// <response code="400">User cannot be approved.</response>
        /// <response code="401">No Access.</response>
        /// <response code="403">No Permission.</response>
        /// <response code="404">Group does not exist.</response>
        [Authorize]
        [Route("api/Groups/{groupSlug}/ApprovedMembers")]
        [CallerMustBeGroupAdministrator]
        [HttpPost]
        public async Task<ActionResult> Post([FromRoute, Slug]string groupSlug, [FromBody] ApproveMember approveMember)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug);

            var newUser = group.Members.SingleOrDefault(m => m.UserId == new MongoDB.Bson.ObjectId(approveMember.UserSlug));
            if (newUser == null)
            {
                var message = _stringLocalizer.GetString("NOT_A_MEMBER_OF_THE_GROUP").Value;
                var details = new ValidationProblemDetails { Detail = message };
                return ValidationProblem(details);
            }

            if (newUser.IsApproved == true)
            {
                var message = _stringLocalizer.GetString("MEMBER_ALREADY_APPROVED").Value;
                var details = new ValidationProblemDetails { Detail = message };
                return ValidationProblem(details);
            }

            newUser.IsApproved = true;

            await _groupStore.UpdateGroup(group);

            return Ok();
        }
    }
}