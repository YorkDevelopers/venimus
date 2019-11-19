using Microsoft.AspNetCore.Mvc.RazorPages;
using YorkDeveloperEvents.ViewModels;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YorkDeveloperEvents.Pages
{
    public class WhatsOnModel : PageModel
    {
        public ListFutureEvents[] ViewModel { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public WhatsOnModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var json = await httpClient.GetStringAsync("/public/FutureEvents");
            ViewModel = JsonSerializer.Deserialize<ListFutureEvents[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
