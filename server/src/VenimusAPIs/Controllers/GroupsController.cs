using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.Mongo;
using VenimusAPIs.Services;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : BaseController
    {
        private readonly UserStore _userStore;
        private readonly EventStore _eventStore;
        private readonly GroupStore _groupStore;

        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;
        private readonly URLBuilder _urlBuilder;

        public GroupsController(UserStore userStore, EventStore eventStore, GroupStore groupStore, IStringLocalizer<ResourceMessages> stringLocalizer, URLBuilder urlBuilder)
        {
            _userStore = userStore;
            _eventStore = eventStore;
            _groupStore = groupStore;
            _stringLocalizer = stringLocalizer;
            _urlBuilder = urlBuilder;
        }

        /// <summary>
        ///     Allows you to create a new group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/groups
        ///     {
        ///         "slug" : "YorkCodeDojo",
        ///         "name" : "York Cod eDojo",
        ///         "isActive" : true,
        ///         "slackChannelName" : "YorkCodeDojo",
        ///         "logoInBase64" : "1111",
        ///         "description" : "Improve your code by practice."
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the created group</returns>
        /// <response code="201">Success</response>
        /// <response code="401">User is not authorized.</response>
        [Authorize(Roles = "SystemAdministrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] CreateGroup group)
        {
            var duplicateGroup = await _groupStore.RetrieveGroupBySlug(group.Slug).ConfigureAwait(false);
            if (duplicateGroup != null)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.GROUP_ALREADY_EXISTS_WITH_THIS_SLUG).Value;
                ModelState.AddModelError(nameof(CreateGroup.Slug), message);
            }

            duplicateGroup = await _groupStore.RetrieveGroupByName(group.Name).ConfigureAwait(false);
            if (duplicateGroup != null)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.GROUP_ALREADY_EXISTS_WITH_THIS_NAME).Value;
                ModelState.AddModelError(nameof(CreateGroup.Name), message);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var model = new Models.Group
            {
                Description = group.Description,
                IsActive = group.IsActive,
                Logo = Convert.FromBase64String(group.LogoInBase64),
                Name = group.Name,
                SlackChannelName = group.SlackChannelName,
                Slug = group.Slug,
                StrapLine = group.StrapLine,
            };

            await _groupStore.StoreGroup(model).ConfigureAwait(false);

            return CreatedAtRoute("Groups", new { groupSlug = model.Slug }, group);
        }

        /// <summary>
        ///     Allows you to update an existing group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/groups/YorkCodeDojo
        ///     {
        ///         "slug" : "YorkCodeDojo",
        ///         "name" : "York Cod eDojo",
        ///         "isActive" : true,
        ///         "slackChannelName" : "YorkCodeDojo",
        ///         "logoInBase64" : "1111",
        ///         "description" : "Improve your code by practice."
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the created group</returns>
        /// <response code="201">Success</response>
        /// <response code="401">User is not authorized.</response>
        [Authorize(Roles = "SystemAdministrator")]
        [HttpPut]
        [Route("{groupSlug}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put([FromServices] IBus bus, [FromRoute, Slug]string groupSlug, [FromBody] UpdateGroup newDetails)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);

            if (group == null)
            {
                return NotFound();
            }

            var updateEvents = false;

            if (!groupSlug.Equals(newDetails.Slug, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateGroup = await _groupStore.RetrieveGroupBySlug(newDetails.Slug).ConfigureAwait(false);
                if (duplicateGroup != null)
                {
                    var message = _stringLocalizer.GetString(Resources.ResourceMessages.GROUP_ALREADY_EXISTS_WITH_THIS_SLUG).Value;
                    ModelState.AddModelError(nameof(UpdateGroup.Slug), message);
                }

                updateEvents = true;
            }

            if (!group.Name.Equals(newDetails.Name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateGroup = await _groupStore.RetrieveGroupByName(newDetails.Name).ConfigureAwait(false);
                if (duplicateGroup != null)
                {
                    var message = _stringLocalizer.GetString(Resources.ResourceMessages.GROUP_ALREADY_EXISTS_WITH_THIS_NAME).Value;
                    ModelState.AddModelError(nameof(UpdateGroup.Name), message);
                }

                updateEvents = true;
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            group.Description = newDetails.Description;
            group.IsActive = newDetails.IsActive;
            group.Name = newDetails.Name;
            group.SlackChannelName = newDetails.SlackChannelName;
            group.Slug = newDetails.Slug;
            group.StrapLine = newDetails.StrapLine;

            if (!string.IsNullOrWhiteSpace(newDetails.LogoInBase64))
            {
                group.Logo = Convert.FromBase64String(newDetails.LogoInBase64);
            }

            await _groupStore.UpdateGroup(group).ConfigureAwait(false);

            if (updateEvents)
            {
                var groupChangedMessage = new ServiceBus.GroupChangedMessage { GroupId = group.Id.ToString() };
                await bus.Publish(groupChangedMessage).ConfigureAwait(false);
            }

            return NoContent();
        }

        /// <summary>
        ///     Allows you to retrieve the details of a group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo
        ///
        /// </remarks>
        /// <returns>The GetGroup view model</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Group does not exist.</response>
        [Route("{groupSlug}", Name = "Groups")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetGroup>> Get(string groupSlug)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);

            if (group == null)
            {
                return NotFound();
            }

            var canViewMembers = false;
            var canAddEvents = false;
            var canAddMembers = false;
            var canJoinGroup = true;
            var canLeaveGroup = false;
            var canEditGroup = false;

            if (User.Identity.IsAuthenticated)
            {
                var uniqueID = UniqueIDForCurrentUser;
                var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);
                canViewMembers = existingUser.IsApproved;

                var member = group.Members.SingleOrDefault(member => member.UserId == existingUser.Id);
                if (member != null)
                {
                    canAddEvents = member.IsAdministrator;
                    canAddMembers = member.IsAdministrator;
                    canEditGroup = member.IsAdministrator;
                    canJoinGroup = false;
                    canLeaveGroup = true;
                }

                if (!canAddMembers)
                {
                    canAddMembers = UserIsASystemAdministrator;
                }
            }

            var viewModel = new GetGroup
            {
                Description = group.Description,
                Name = group.Name,
                Slug = group.Slug,
                Logo = _urlBuilder.BuildGroupLogoURL(groupSlug),
                IsActive = group.IsActive,
                SlackChannelName = group.SlackChannelName,
                StrapLine = group.StrapLine,
                CanViewMembers = canViewMembers,
                CanAddEvents = canAddEvents,
                CanAddMembers = canAddMembers,
                CanJoinGroup = canJoinGroup,
                CanLeaveGroup = canLeaveGroup,
                CanEditGroup = canEditGroup,
            };

            return viewModel;
        }

        /// <summary>
        ///     Allows you to retrieve the list of all groups
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups
        ///
        /// </remarks>
        /// <returns>An array of ListGroup view models</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListGroups[]>> Get([FromServices] URLBuilder groupLogoURLBuilder, [FromQuery] bool includeInActiveGroups = false, [FromQuery] bool groupsIBelongToOnly = false)
        {
            var filters = Builders<Group>.Filter.Empty;

            if (!includeInActiveGroups)
            {
                filters &= Builders<Group>.Filter.Eq(ent => ent.IsActive, true);
            }

            if (groupsIBelongToOnly)
            {
                var uniqueID = UniqueIDForCurrentUser;
                var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);
                var memberMatch = Builders<GroupMember>.Filter.Eq(a => a.UserId, existingUser.Id);
                filters &= Builders<Group>.Filter.ElemMatch(x => x.Members, memberMatch);
            }

            var groups = await _groupStore.RetrieveAllGroups(filters).ConfigureAwait(false);

            var viewModels = groups.Select(grp => new ListGroups
            {
                StrapLine = grp.StrapLine,
                Description = grp.Description,
                IsActive = grp.IsActive,
                Name = grp.Name,
                SlackChannelName = grp.SlackChannelName,
                Slug = grp.Slug,
                Logo = groupLogoURLBuilder.BuildGroupLogoURL(grp.Slug),
            }).ToArray();

            return viewModels;
        }

        /// <summary>
        ///     Allows you to get the logo for a group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YORKCODEDOJO/logo
        ///
        /// </remarks>
        /// <returns>byte array for the image</returns>
        /// <response code="200">Success</response>
        [Route("/api/groups/{groupSlug}/logo")]
        [HttpGet]
        [ResponseCache(Duration = 300)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLogo([FromRoute, Slug] string groupSlug)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);
            if (group == null)
            {
                return NotFound();
            }

            return File(group.Logo, "image/jpeg");
        }

        /// <summary>
        ///     Allows you to delete an existing group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/groups/YorkCodeDojo
        ///
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="204">NoContent</response>
        /// <response code="401">User is not authorized.</response>
        [Authorize(Roles = "SystemAdministrator")]
        [HttpDelete]
        [Route("{groupSlug}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] string groupSlug)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);

            if (group != null)
            {
                var eventsExistForGroup = await _eventStore.DoEventsExistForGroup(groupSlug).ConfigureAwait(false);
                if (eventsExistForGroup)
                {
                    var message = _stringLocalizer.GetString(Resources.ResourceMessages.GROUP_HAS_EVENTS).Value;
                    var details = new ValidationProblemDetails { Detail = message };
                    return ValidationProblem(details);
                }

                await _groupStore.DeleteGroup(group).ConfigureAwait(false);
            }

            return NoContent();
        }
    }
}