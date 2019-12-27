using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Questions = AddDummyQuestions(existingDetails.Questions.ToList()),
            };

            return Page();
        }

        private List<Question> AddDummyQuestions(List<Question> list)
        {
            for (int i = 0; i < 5; i++)
                list.Add(new Question { QuestionType = "Text", Code = Guid.NewGuid().ToString() });

            return list;
        }

        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            if (!ModelState.IsValid) return Page();

            UpdateEvent.Questions = UpdateEvent.Questions
                                               .Where(q => !string.IsNullOrWhiteSpace(q.Caption))
                                               .Select(q => new Question { Caption = q.Caption, Code = q.Code, QuestionType = "Text" })
                                               .ToList();

            var result = await api.UpdateEvent(GroupSlug, EventSlug, UpdateEvent);

            return result.Evalulate(
                        onSuccess: () => LocalRedirect("/"),
                        onFailure: validationProblemDetails =>
                        {
                            UpdateEvent.Questions = AddDummyQuestions(UpdateEvent.Questions);
                            return AddProblemsToModelState(validationProblemDetails, nameof(UpdateEvent));
                        });
        }
    }
}