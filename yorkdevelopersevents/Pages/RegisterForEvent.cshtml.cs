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

        [BindProperty(SupportsGet = true)]
        public string GroupSlug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EventSlug { get; set; }

        [BindProperty]
        public bool CurrentlyRegistered { get; set; }

        public GetEvent Event;

        public async Task OnGet([FromServices] API api)
        {
            await api.JoinGroup(GroupSlug);

            Event = await api.GetEvent(GroupSlug, EventSlug);

            var currentRegistration = await api.GetEventRegistrationDetails(GroupSlug, EventSlug);

            if (currentRegistration != null)
            {
                RegisterForEvent = new RegisterForEvent
                {
                    DietaryRequirements = currentRegistration.DietaryRequirements,
                    MessageToOrganiser = currentRegistration.MessageToOrganiser,
                    NumberOfGuests = currentRegistration.NumberOfGuests,
                };

                CurrentlyRegistered = currentRegistration.Attending;
            }
        }

        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            await api.RegisterForEvent(GroupSlug, EventSlug, RegisterForEvent);

            return LocalRedirect("/MyEvents");
        }
    }
}