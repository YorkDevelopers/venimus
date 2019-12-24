using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class CreateGroupModel : BasePageModel
    {
        [BindProperty]
        public CreateGroup CreateGroup { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            if (!ModelState.IsValid) return Page();

            using var ms = new MemoryStream();
            Upload.CopyTo(ms);
            CreateGroup.LogoInBase64 = Convert.ToBase64String(ms.ToArray());

            var result = await api.CreateGroup(CreateGroup);

            return result.Evalulate(
                        onSuccess: () => LocalRedirect("/"),
                        onFailure: validationProblemDetails => AddProblemsToModelState(validationProblemDetails, nameof(CreateGroup)));
        }
    }
}