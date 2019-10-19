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

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] CreateNewGroup group)
        {
            var model = _mapper.Map<Models.Group>(group);

            await _mongo.StoreGroup(model);

            return CreatedAtRoute("Groups", new { groupName = model.Name }, group);
        }

        [Authorize]
        [Route("{groupName}", Name = "Groups")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetGroup>> Get(string groupName)
        {
            var group = await _mongo.RetrieveGroup(groupName);

            if (group == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<GetGroup>(group);

            return viewModel;
        }
    }
}