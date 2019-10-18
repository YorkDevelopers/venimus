using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly Services.Mongo _mongo;

        public GroupController(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateNewGroup group)
        {
            var model = new Models.Group();

            await _mongo.StoreGroup(model);

            return Ok();
        }
    }
}