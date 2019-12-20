using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
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

        internal async Task<ViewMyDetails> GetCurrentUser()
        {
            var client = await Client();
            return await client.GetAsJson<ViewMyDetails>($"/api/User");
        }

        internal async Task CreateEvent(string groupSlug, CreateEvent createEvent)
        {
            var client = await Client();
            var response = await client.PostAsJsonAsync($"/api/groups/{groupSlug}/events", createEvent);
            var text = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        internal async Task UpdateUser(UpdateMyDetails updatedDetails)
        {
            var client = await Client();
            var response = await client.PutAsJsonAsync($"/api/User", updatedDetails);
            var text = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        internal async Task<GetEvent> GetEvent(string groupSlug, string eventSlug)
        {
            var client = await Client();
            return await client.GetAsJson<GetEvent>($"/api/Groups/{groupSlug}/Events/{eventSlug}");
        }

        internal async Task<GetGroup> GetGroup(string groupSlug)
        {
            var client = await Client();
            return await client.GetAsJson<GetGroup>($"/api/Groups/{groupSlug}");
        }

        public API(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        internal async Task RegisterForEvent(string groupSlug, string eventSlug, RegisterForEvent registerForEvent)
        {
            var client = await Client();

            var response = await client.PutAsJsonAsync($"api/User/Groups/{groupSlug}/Events/{eventSlug}", registerForEvent);
            response.EnsureSuccessStatusCode();
        }

        internal async Task UnRegisterFromEvent(string groupSlug, string eventSlug)
        {
            var client = await Client();

            var response = await client.DeleteAsync($"api/user/groups/{groupSlug}/Events/{eventSlug}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<ListGroups[]> ListGroups(bool includeInActiveGroups, bool groupsIBelongToOnly)
        {
            var client = await Client();
            return await client.GetAsJson<ListGroups[]>($"/api/groups?includeInActiveGroups={includeInActiveGroups}&groupsIBelongToOnly={groupsIBelongToOnly}");
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

        internal async Task<ViewAllMyEventRegistrations[]> ListMyEvents()
        {
            var client = await Client();
            return await client.GetAsJson<ViewAllMyEventRegistrations[]>("api/user/events");
        }

        internal async Task<ViewMyEventRegistration> GetEventRegistrationDetails(string groupSlug, string eventSlug)
        {
            var client = await Client();
            return await client.GetAsJson<ViewMyEventRegistration>($"api/user/groups/{groupSlug}/events/{eventSlug}");
        }
    }
}
