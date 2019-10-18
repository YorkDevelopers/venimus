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
    public class GroupController : ControllerBase
    {
        private readonly Services.Mongo _mongo;

        private readonly IMapper _mapper;

        public GroupController(Services.Mongo mongo, IMapper mapper)
        {
            _mongo = mongo;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateNewGroup group)
        {
            var model = _mapper.Map<Models.Group>(group);

            await _mongo.StoreGroup(model);

            return CreatedAtRoute("Groups", new { groupName = model.Name }, group);
        }

        [Route("{groupName}", Name = "Groups")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Get(string groupName)
        {
            return NotFound();
        }
    }
}