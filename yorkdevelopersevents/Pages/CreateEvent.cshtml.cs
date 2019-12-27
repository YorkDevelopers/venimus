using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class CreateEventModel : BasePageModel
    {
        [BindProperty(SupportsGet = true)]
        public string GroupSlug { get; set; }

        [BindProperty]
        public CreateEvent CreateEvent { get; set; }

        public void OnGet()
        {
            if (CreateEvent == null)
            {
                CreateEvent = new CreateEvent
                {
                    Questions = new List<Question>()
                };

                CreateEvent.Questions = AddDummyQuestions(CreateEvent.Questions);
            }
        }


        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            if (!ModelState.IsValid) return Page();

            CreateEvent.Questions = CreateEvent.Questions
                                               .Where(q => !string.IsNullOrWhiteSpace(q.Caption))
                                               .Select(q => new Question { Caption = q.Caption, Code = q.Code, QuestionType = "Text" })
                                               .ToList();

            var result = await api.CreateEvent(GroupSlug, CreateEvent);

            return result.Evalulate(
                        onSuccess: () => LocalRedirect("/"),
                        onFailure: validationProblemDetails =>
                        {
                            CreateEvent.Questions = AddDummyQuestions(CreateEvent.Questions);
                            return AddProblemsToModelState(validationProblemDetails, nameof(UpdateEvent));
                        });
        }

        private List<Question> AddDummyQuestions(List<Question> list)
        {
            for (int i = 0; i < 5; i++)
                list.Add(new Question { QuestionType = "Text", Code = Guid.NewGuid().ToString() });

            return list;
        }
    }
}