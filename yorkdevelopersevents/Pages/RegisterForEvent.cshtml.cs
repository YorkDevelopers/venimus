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
        [BindProperty(SupportsGet = true)]
        public string GroupSlug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EventSlug { get; set; }

        [BindProperty]
        public bool CurrentlyRegistered { get; set; }

        [BindProperty] 
        public ViewMyEventRegistration CurrentRegistration { get; set; }

        public async Task OnGet([FromServices] API api)
        {
            await api.JoinGroup(GroupSlug);

            CurrentRegistration = await api.GetEventRegistrationDetails(GroupSlug, EventSlug);

            CurrentlyRegistered = CurrentRegistration.Attending;

        }

        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            var registerForEvent = new RegisterForEvent
            {
                Answers = CurrentRegistration.Answers,
            };

            await api.RegisterForEvent(GroupSlug, EventSlug, registerForEvent);

            return LocalRedirect("/MyEvents");
        }
    }
}