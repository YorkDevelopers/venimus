using Microsoft.AspNetCore.Mvc.RazorPages;
using YorkDeveloperEvents.ViewModels;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
            ViewModel = JsonSerializer.Deserialize<ListActiveGroups[]>(publicGroupsJSON).Where(g => !string.IsNullOrEmpty(g.name)).ToArray();
        }

        public async Task OnPost()
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var publicGroupsJSON = await httpClient.GetStringAsync("/public/Groups");
            ViewModel = JsonSerializer.Deserialize<ListActiveGroups[]>(publicGroupsJSON).Where(g => !string.IsNullOrEmpty(g.name)).ToArray();
        }
    }
}
