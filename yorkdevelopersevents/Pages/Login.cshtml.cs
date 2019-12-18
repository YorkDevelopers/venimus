using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;

namespace YorkDeveloperEvents.Pages
{
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnGet([FromServices] API api, string ReturnURL = "/", string Mode = "Challenge")
        {
            if (Mode == "Challenge")
            {
                await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = "/Login?Mode=CheckIfRegistered&ReturnURL=" + ReturnURL });
                return Page();
            }
            else
            {
                var userDetails = await api.GetCurrentUser();
                if (!userDetails.IsRegistered)
                {
                    return LocalRedirect("/NewUser");
                }
            }

            return LocalRedirect(ReturnURL);
        }
    }
}