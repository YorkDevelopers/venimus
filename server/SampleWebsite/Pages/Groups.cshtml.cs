using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleWebsite.Extensions;

namespace SampleWebsite.Pages
{
    public class GroupsModel : PageModel
    {
        public GroupViewModel ViewModel { get; set; }

        public async Task OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("Auth0", "access_token");

            var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://venimus-api.azurewebsites.net");
            httpClient.BaseAddress = new Uri("https://localhost:7001");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var json = await httpClient.GetStringAsync("/api/Groups/YorkCodeDojo");
            ViewModel = JsonSerializer.Deserialize<GroupViewModel>(json);
        }

        private void CreateGroup()
        {
            //var yd = new GroupViewModel
            //{
            //    Name = "YorkCodeDojo",
            //    Description = "Practice, Share, Learn, Teach",
            //};
            //var response = await httpClient.PostAsJsonAsync("/api/Groups", yd);
            //response.EnsureSuccessStatusCode();
        }

        public class GroupViewModel
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }
        }
    }
}