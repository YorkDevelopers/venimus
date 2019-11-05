using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly Services.Mongo _mongo;

        private readonly IMapper _mapper;

        public GroupsController(Services.Mongo mongo, IMapper mapper)
        {
            _mongo = mongo;
            _mapper = mapper;
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
            var duplicateGroup = await _mongo.RetrieveGroupBySlug(group.Slug);
            if (duplicateGroup != null)
            {
                ModelState.AddModelError("Slug", "A group using this slug already exists");
            }

            duplicateGroup = await _mongo.RetrieveGroupByName(group.Name);
            if (duplicateGroup != null)
            {
                ModelState.AddModelError("Name", "A group using this name already exists");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var model = _mapper.Map<Models.Group>(group);

            await _mongo.StoreGroup(model);

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
        public async Task<IActionResult> Put([FromRoute]string groupSlug, [FromBody] UpdateGroup newDetails)
        {
            var group = await _mongo.RetrieveGroupBySlug(groupSlug);

            if (group == null)
            {
                return NotFound();
            }

            if (!groupSlug.Equals(newDetails.Slug, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateGroup = await _mongo.RetrieveGroupBySlug(newDetails.Slug);
                if (duplicateGroup != null)
                {
                    ModelState.AddModelError("Slug", "A group using this slug already exists");
                }
            }

            if (!group.Name.Equals(newDetails.Name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicateGroup = await _mongo.RetrieveGroupByName(newDetails.Name);
                if (duplicateGroup != null)
                {
                    ModelState.AddModelError("Name", "A group using this name already exists");
                }
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(newDetails, group);

            await _mongo.UpdateGroup(group);

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
            var group = await _mongo.RetrieveGroupBySlug(groupSlug);

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
            var groups = await _mongo.RetrieveAllGroups();

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
            var group = await _mongo.RetrieveGroupBySlug(groupSlug);

            if (group != null)
            {
                var eventsExistForGroup = await _mongo.DoEventsExistForGroup(groupSlug);
                if (eventsExistForGroup)
                {
                    var details = new ValidationProblemDetails { Detail = "The group cannot be deleted as it has one or events.  Please mark the group as InActive instead." };
                    return ValidationProblem(details);
                }

                await _mongo.DeleteGroup(group);
            }

            return NoContent();
        }
    }
}