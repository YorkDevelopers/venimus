using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class EditEventModel : BasePageModel
    {
        [BindProperty(SupportsGet = true)]
        public string GroupSlug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EventSlug { get; set; }

        [BindProperty]
        public UpdateEvent UpdateEvent { get; set; }

        public async Task<ActionResult> OnGet([FromServices] API api)
        {
            var existingDetails = await api.GetEvent(GroupSlug, EventSlug);

            UpdateEvent = new UpdateEvent
            {
                Description = existingDetails.EventDescription,
                EndTimeUTC = existingDetails.EventFinishesUTC,
                GuestsAllowed = existingDetails.GuestsAllowed,
                Location = existingDetails.EventLocation,
                MaximumNumberOfAttendees = existingDetails.MaximumNumberOfAttendees,
                Slug = EventSlug,
                StartTimeUTC = existingDetails.EventStartsUTC,
                Title = existingDetails.EventTitle,
            };

            return Page();
        }


        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            if (!ModelState.IsValid) return Page();

            var result = await api.UpdateEvent(GroupSlug, EventSlug, UpdateEvent);

            return result.Evalulate(
                        onSuccess: () => LocalRedirect("/"),
                        onFailure: validationProblemDetails => AddProblemsToModelState(validationProblemDetails, nameof(UpdateEvent)));
        }
    }
}