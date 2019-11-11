using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleWebsite.Extensions;
using VenimusAPIs.ViewModels;

namespace SampleWebsite.Pages
{
    public class GroupsModel : PageModel
    {
        public CreateGroup ViewModel { get; set; }

        public async Task OnGetAsync()
        {
            var me = User;

            var accessToken = await HttpContext.GetTokenAsync("Auth0", "access_token");

            var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://venimus-api.azurewebsites.net");
            httpClient.BaseAddress = new Uri("https://localhost:7001");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var r = await httpClient.PostAsync("/api/users/Connected", null);

            var yd = new CreateGroup
            {
                Name = "York Code Dojo",
                Slug = "YorkCodeDojo",
                Description = "Practice, Share, Learn, Teach",
            };
            var response = await httpClient.PostAsJsonAsync("/api/Groups", yd);
            response.EnsureSuccessStatusCode();
            var loc = response.Headers.Location;

            var json = await httpClient.GetStringAsync("/api/Groups/YorkCodeDojo");
            ViewModel = JsonSerializer.Deserialize<CreateGroup>(json);

            var ug = new UpdateGroup { Slug = "Change", Name="Change", Description="Change" };
            response = await httpClient.PutAsJsonAsync(loc.ToString(), ug);
            response.EnsureSuccessStatusCode();
        }
    }
}