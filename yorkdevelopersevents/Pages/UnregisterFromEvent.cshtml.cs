using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;

namespace YorkDeveloperEvents
{
    public class UnregisterFromEventModel : PageModel
    {
        public async Task<ActionResult> OnPost([FromServices] API api, string GroupSlug, string EventSlug)
        {
            await api.UnRegisterFromEvent(GroupSlug, EventSlug);

            return LocalRedirect("/MyEvents");
        }
    }
}