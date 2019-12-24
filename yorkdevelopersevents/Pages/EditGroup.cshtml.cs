using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class EditGroupModel : BasePageModel
    {
        [BindProperty(SupportsGet = true)]
        public string GroupSlug { get; set; }

        [BindProperty]
        public UpdateGroup UpdateGroup { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task<ActionResult> OnGet([FromServices] API api)
        {
            var existingDetails = await api.GetGroup(GroupSlug);

            UpdateGroup = new UpdateGroup
            {
                Description = existingDetails.Description,
                IsActive = existingDetails.IsActive,
                Name = existingDetails.Name,
                SlackChannelName = existingDetails.SlackChannelName,
                StrapLine = existingDetails.StrapLine,
                Slug = existingDetails.Slug,
            };

            return Page();
        }


        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            if (!ModelState.IsValid) return Page();

            if (Upload != null)
            {
                using var ms = new MemoryStream();
                Upload.CopyTo(ms);
                UpdateGroup.LogoInBase64 = Convert.ToBase64String(ms.ToArray());
            }

            var result = await api.UpdateGroup(GroupSlug, UpdateGroup);

            return result.Evalulate(
                        onSuccess: () => LocalRedirect("/"),
                        onFailure: validationProblemDetails => AddProblemsToModelState(validationProblemDetails, nameof(UpdateGroup)));
        }
    }
}