using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YorkDeveloperEvents.Extensions;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents.Services
{
    public class API
    {
        private readonly HttpClient _client;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public API(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ListMyGroups[]> ListMyGroups()
        {
            var client = await Client();
            return await client.GetAsJson<ListMyGroups[]>("/api/user/groups");
        }

        private async Task<HttpClient> Client()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("Auth0", "access_token");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return _client;
        }

        public async Task JoinGroup(string groupSlug)
        {
            var client = await Client();

            var data = new { GroupSlug = groupSlug };
            var response = await client.PostAsJsonAsync($"api/User/Groups", data);

            response.EnsureSuccessStatusCode();
        }

        public async Task LeaveGroup(string groupSlug)
        {
            var client = await Client();
            var response = await client.DeleteAsync($"api/User/Groups/{groupSlug}");

            response.EnsureSuccessStatusCode();
        }

        public async Task<ListGroupMembers[]> ListGroupMembers(string groupSlug)
        {
            var client = await Client();
            return await client.GetAsJson<ListGroupMembers[]>($"/api/Groups/{groupSlug}/Members");
        }
    }
}
