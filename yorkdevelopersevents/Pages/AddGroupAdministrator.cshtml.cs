using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class AddGroupAdministratorModel : BasePageModel
    {
        [Required]
        [BindProperty]
        public string DisplayName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GroupSlug { get; set; }


        public async Task<ActionResult> OnPost([FromServices] API api)
        {
            if (!ModelState.IsValid) return Page();

            var theUser = await api.GetUsersDetails(DisplayName);
            if (theUser == null)
            {
                ModelState.AddModelError(nameof(DisplayName), "The user does not exist");
                return Page();
            }

            var addGroupMember = new AddGroupMember
            {
                Slug = theUser.Slug,
                IsAdministrator = true,
            };

            var result = await api.AddGroupMember(GroupSlug, addGroupMember);

            return result.Evalulate(
                        onSuccess: () => LocalRedirect("/"),
                        onFailure: validationProblemDetails => AddProblemsToModelState(validationProblemDetails, ""));
        }
    }
}