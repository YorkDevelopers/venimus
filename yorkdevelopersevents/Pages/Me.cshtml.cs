using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class MeModel : PageModel
    {
        [BindProperty]
        public UpdateMyDetails UpdatedDetails { get; set; }

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
            await api.UpdateUser(UpdatedDetails);

            return LocalRedirect("/");
        }
    }
}