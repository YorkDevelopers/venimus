using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class ActiveGroupsController : ControllerBase
    {
        private readonly Mongo.GroupStore _groupStore;

        private readonly IMapper _mapper;

        public ActiveGroupsController(Mongo.GroupStore groupStore, IMapper mapper)
        {
            _groupStore = groupStore;
            _mapper = mapper;
        }

        /// <summary>
        ///     Allows you request the list of active groups
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /public/Groups
        ///
        /// </remarks>
        /// <returns>Array of ListActiveGroups view models</returns>
        /// <response code="200">Success</response>
        [Route("public/Groups")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListActiveGroups[]>> Get()
        {
            var groups = await _groupStore.GetActiveGroups();

            var server = $"{Request.Scheme}://{Request.Host}";

            var viewModels = groups.Select(grp => new ListActiveGroups
            {
                Description = grp.Description,
                Name = grp.Name,
                Slug = grp.Slug,
                Logo = $"{server}/api/groups/{grp.Slug}/logo",
            }).ToArray();

            return viewModels;
        }
    }
}