using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public abstract class BaseUserController : ControllerBase
    {
        protected string UniqueIDForCurrentUser => (User.Identity as ClaimsIdentity).Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
