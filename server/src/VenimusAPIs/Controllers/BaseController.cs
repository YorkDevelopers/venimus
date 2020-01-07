using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected string UniqueIDForCurrentUser => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        protected bool UserIsASystemAdministrator => User.IsInRole("SystemAdministrator");
    }
}
