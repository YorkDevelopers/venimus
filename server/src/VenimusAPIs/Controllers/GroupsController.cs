﻿using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.GroupStore _groupStore;

        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Messages> _stringLocalizer;

        public GroupsController(Mongo.EventStore eventStore, Mongo.GroupStore groupStore, IMapper mapper, IStringLocalizer<Messages> stringLocalizer)
        {
            _eventStore = eventStore;
            _groupStore = groupStore;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
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
            var duplicateGroup = await _groupStore.RetrieveGroupBySlug(group.Slug);
            if (duplicateGroup != null)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.GROUP_ALREADY_EXISTS_WITH_THIS_SLUG).Value;
                ModelState.AddModelError(nameof(CreateGroup.Slug), message);
            }

            duplicateGroup = await _groupStore.RetrieveGroupByName(group.Name);
            if (duplicateGroup != null)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.GROUP_ALREADY_EXISTS_WITH_THIS_NAME).Value;
                ModelState.AddModelError(nameof(CreateGroup.Name), message);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var model = _mapper.Map<Models.Group>(group);

            await _groupStore.StoreGroup(model);

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
        public async Task<IActionResult> Put([FromServices] IBus bus, [FromRoute]string groupSlug, [FromBody] UpdateGroup newDetails)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug);

            if (group == null)
            {
                return NotFound();
            }

            var updateEvents = false;

            if (!groupSlug.Equals(newDetails.Slug, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateGroup = await _groupStore.RetrieveGroupBySlug(newDetails.Slug);
                if (duplicateGroup != null)
                {
                    var message = _stringLocalizer.GetString(Resources.Messages.GROUP_ALREADY_EXISTS_WITH_THIS_SLUG).Value;
                    ModelState.AddModelError(nameof(UpdateGroup.Slug), message);
                }

                updateEvents = true;
            }

            if (!group.Name.Equals(newDetails.Name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateGroup = await _groupStore.RetrieveGroupByName(newDetails.Name);
                if (duplicateGroup != null)
                {
                    var message = _stringLocalizer.GetString(Resources.Messages.GROUP_ALREADY_EXISTS_WITH_THIS_NAME).Value;
                    ModelState.AddModelError(nameof(UpdateGroup.Name), message);
                }

                updateEvents = true;
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(newDetails, group);

            await _groupStore.UpdateGroup(group);

            if (updateEvents)
            {
                var groupChangedMessage = new ServiceBusMessages.GroupChangedMessage { GroupId = group.Id.ToString() };
                await bus.Publish(groupChangedMessage);
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
        [Authorize]
        [Route("{groupSlug}", Name = "Groups")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetGroup>> Get(string groupSlug)
        {
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug);

            if (group == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<GetGroup>(group);

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
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListGroups[]>> Get()
        {
            var groups = await _groupStore.RetrieveAllGroups();

            var viewModels = _mapper.Map<ListGroups[]>(groups);

            return viewModels;
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
            var group = await _groupStore.RetrieveGroupBySlug(groupSlug);

            if (group != null)
            {
                var eventsExistForGroup = await _eventStore.DoEventsExistForGroup(groupSlug);
                if (eventsExistForGroup)
                {
                    var message = _stringLocalizer.GetString(Resources.Messages.GROUP_HAS_EVENTS).Value;
                    var details = new ValidationProblemDetails { Detail = message };
                    return ValidationProblem(details);
                }

                await _groupStore.DeleteGroup(group);
            }

            return NoContent();
        }
    }
}