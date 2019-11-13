using System.Collections.Generic;
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
        public async Task<ActionResult<List<ListActiveGroups>>> Get()
        {
            var groups = await _groupStore.GetActiveGroups();

            var viewModels = _mapper.Map<List<ListActiveGroups>>(groups);

            return viewModels;
        }
    }
}