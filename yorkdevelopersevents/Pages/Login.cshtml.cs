using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace YorkDeveloperEvents.Pages
{
    public class LoginModel : PageModel
    {
        public async Task OnGet()
        {
            var returnUrl = "https://" + Request.Host + Request.PathBase + "/";
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }
    }
}