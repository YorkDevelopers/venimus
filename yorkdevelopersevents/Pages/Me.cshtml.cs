using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class MeModel : BasePageModel
    {
        [BindProperty]
        public UpdateMyDetails UpdatedDetails { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task OnGet([FromServices] API api)
        {
            var existingDetails = await api.GetCurrentUser();
            UpdatedDetails = new UpdateMyDetails
            {
                Bio = existingDetails.Bio,
                DisplayName = existingDetails.DisplayName,
                Fullname = existingDetails.Fullname,
                Pronoun = existingDetails.Pronoun,
            };
        }

        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            if (!ModelState.IsValid) return Page();

            if (Upload != null)
            {
                using var ms = new MemoryStream();
                Upload.CopyTo(ms);
                UpdatedDetails.ProfilePictureAsBase64 = Convert.ToBase64String(ms.ToArray());
            }

            var result = await api.UpdateUser(UpdatedDetails);

            return result.Evalulate(
                        onSuccess: () => LocalRedirect("/"),
                        onFailure: validationProblemDetails => AddProblemsToModelState(validationProblemDetails, nameof(UpdateMyDetails)));
        }
    }
}