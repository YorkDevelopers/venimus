using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YorkDeveloperEvents.Extensions;

namespace YorkDeveloperEvents.Pages
{
    [Authorize]
    public class JoinGroupModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public JoinGroupModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGet(string groupSlug)
        {
            var accessToken = await HttpContext.GetTokenAsync("Auth0", "access_token");
            var httpClient = _httpClientFactory.CreateClient("API");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var data = new { GroupSlug = groupSlug };
            var response = await httpClient.PostAsJsonAsync($"api/User/Groups", data);

            response.EnsureSuccessStatusCode();
        }
    }
}