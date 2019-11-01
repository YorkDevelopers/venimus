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
        private readonly Services.Mongo _mongo;
        
        private readonly IMapper _mapper;

        public ActiveGroupsController(Services.Mongo mongo, IMapper mapper)
        {
            _mongo = mongo;
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
            var groups = await _mongo.GetActiveGroups();

            var viewModels = _mapper.Map<List<ListActiveGroups>>(groups);

            return viewModels;
        }
    }
}