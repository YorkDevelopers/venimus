using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    [Authorize]
    public class RegisterForEventModel : PageModel
    {
        [BindProperty]
        public RegisterForEvent RegisterForEvent { get; set; }

        [BindProperty]
        public string GroupSlug { get; set; }

        public GetEvent Event;

        public async Task OnGet([FromServices] API api, string groupSlug, string eventSlug)
        {
            await api.JoinGroup(groupSlug);

            Event = await api.GetEvent(groupSlug, eventSlug);

            GroupSlug = groupSlug;
        }

        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            await api.RegisterForEvent(GroupSlug, RegisterForEvent);

            return LocalRedirect("/MyEvents");

        }
    }
}