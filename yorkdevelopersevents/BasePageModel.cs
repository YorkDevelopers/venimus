using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace YorkDeveloperEvents
{
    [Authorize]
    public abstract class BasePageModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasePageModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected async Task<HttpClient> APIClient()
        {
            var accessToken = await HttpContext.GetTokenAsync("Auth0", "access_token");
            var httpClient = _httpClientFactory.CreateClient("API");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return httpClient;
        }
    }
}
