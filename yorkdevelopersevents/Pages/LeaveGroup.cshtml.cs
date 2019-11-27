using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;

namespace YorkDeveloperEvents.Pages
{
    [Authorize]
    public class LeaveGroupModel : PageModel
    {
        public async Task<ActionResult> OnPost([FromServices] API api, string groupSlug)
        {
            await api.LeaveGroup(groupSlug);
            return LocalRedirect("/MyGroups");
        }
    }
}
