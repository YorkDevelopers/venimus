using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace YorkDeveloperEvents.Pages
{
    public class LoginModel : PageModel
    {
        public async Task OnGet(string ReturnURL = "/")
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = ReturnURL });
        }
    }
}