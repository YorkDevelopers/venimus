using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YorkDeveloperEvents.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace YorkDeveloperEvents.Pages
{
    public class GroupsModel : PageModel
    {
        public ListActiveGroups[] ViewModel { get; set; }

        public async Task OnGetAsync()
        {
            var me = User;

            var accessToken = await HttpContext.GetTokenAsync("Auth0", "access_token");

            var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://venimus-api.azurewebsites.net");
            httpClient.BaseAddress = new Uri("https://localhost:7001");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //var yd = new CreateGroup
            //{
            //    Name = "York Code Dojo3",
            //    Slug = "YorkCodeDojo3",
            //    Description = "Practice, Share, Learn, Teach",
            //    LogoInBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@"C:\code\venimus\server\tests\VenimusAPIs.Tests\images\York_Code_Dojo.jpg")),
            //    IsActive = true,
            //};
            //var response = await httpClient.PostAsJsonAsync("/api/Groups", yd);
            //response.EnsureSuccessStatusCode();

            var publicGroupsJSON = await httpClient.GetStringAsync("/public/Groups");
            ViewModel = JsonSerializer.Deserialize<ListActiveGroups[]>(publicGroupsJSON);

            //var r = await httpClient.PostAsync("/api/users/Connected", null);


            //var loc = response.Headers.Location;

            //var json = await httpClient.GetStringAsync("/api/Groups/YorkCodeDojo");
            //ViewModel = JsonSerializer.Deserialize<CreateGroup>(json);

            //var ug = new UpdateGroup { Slug = "Change", Name="Change", Description="Change" };
            //response = await httpClient.PutAsJsonAsync(loc.ToString(), ug);
            //response.EnsureSuccessStatusCode();
        }
    }
}