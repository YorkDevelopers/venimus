using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents.Pages
{
    public class IndexModel : PageModel
    {
        public ListActiveGroups[] ViewModel { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var publicGroupsJSON = await httpClient.GetStringAsync("/public/Groups");
            ViewModel = JsonSerializer.Deserialize<ListActiveGroups[]>(publicGroupsJSON, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
