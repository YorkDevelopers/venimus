using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;

namespace YorkDeveloperEvents.Pages
{
    public class JoinGroupModel : PageModel
    {
        public async Task<ActionResult> OnGet([FromServices] API api, string groupSlug)
        {
            await api.JoinGroup(groupSlug);

            return LocalRedirect("/MyGroups");
        }
    }
}