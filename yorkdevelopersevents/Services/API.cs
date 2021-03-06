﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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

        internal async Task<APIResult> AddGroupMember(string groupSlug, AddGroupMember addGroupMember)
        {
            var client = await Client();
            var response = await client.PostAsJsonAsync($"api/Groups/{groupSlug}/Members", addGroupMember);
            return await APIResult.Create(response);
        }

        internal async Task<APIResult> CreateGroup(CreateGroup createGroup)
        {
            var client = await Client();
            var response = await client.PostAsJsonAsync($"/api/groups", createGroup);
            return await APIResult.Create(response);
        }

        internal async Task<APIResult> UpdateGroup(string groupSlug, UpdateGroup updateGroup)
        {
            var client = await Client();
            var response = await client.PutAsJsonAsync($"/api/groups/{groupSlug}", updateGroup);
            return await APIResult.Create(response);
        }

        internal async Task<APIResult> CreateEvent(string groupSlug, CreateEvent createEvent)
        {
            var client = await Client();
            var response = await client.PostAsJsonAsync($"/api/groups/{groupSlug}/events", createEvent);
            return await APIResult.Create(response);
        }

        internal async Task<APIResult> UpdateEvent(string groupSlug, string eventSlug, UpdateEvent updateEvent)
        {
            var client = await Client();
            var response = await client.PutAsJsonAsync($"/api/groups/{groupSlug}/events/{eventSlug}", updateEvent);
            return await APIResult.Create(response);
        }

        internal async Task<APIResult> UpdateUser(UpdateMyDetails updatedDetails)
        {
            var client = await Client();
            var response = await client.PutAsJsonAsync($"/api/User", updatedDetails);
            return await APIResult.Create(response);
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

        internal async Task<GetUser> GetUsersDetails(string displayName)
        {
            var client = await Client();
            var response = await client.GetAsync($"/api/Users?DisplayName={displayName}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            var dataAsString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GetUser>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        internal async Task<ViewMyDetails> GetCurrentUser()
        {
            var client = await Client();
            return await client.GetAsJson<ViewMyDetails>($"/api/User");
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

        public async Task<ListGroups[]> ListGroups(bool includeInActiveGroups, bool groupsIBelongToOnly)
        {
            var client = await Client();
            return await client.GetAsJson<ListGroups[]>($"/api/groups?includeInActiveGroups={includeInActiveGroups}&groupsIBelongToOnly={groupsIBelongToOnly}");
        }

        public async Task<ListGroupMembers[]> ListGroupMembers(string groupSlug)
        {
            var client = await Client();
            return await client.GetAsJson<ListGroupMembers[]>($"/api/Groups/{groupSlug}/Members");
        }

        public async Task<ListEventsForGroup[]> ListEventsForGroup(string groupSlug, bool includePastEvents, bool includeFutureEvents)
        {
            var client = await Client();
            return await client.GetAsJson<ListEventsForGroup[]>($"/api/Groups/{groupSlug}/Events?includePastEvents={includePastEvents}&includeFutureEvents={includeFutureEvents}");
        }

        internal async Task<ListEvents[]> ListMyEvents()
        {
            var client = await Client(tokenRequired: true);
            return await client.GetAsJson<ListEvents[]>("api/events?EventsIHaveSignedUpToOnly=true");
        }

        internal async Task<ListEvents[]> ListEvents()
        {
            var client = await Client(tokenRequired: false);
            return await client.GetAsJson<ListEvents[]>("api/events");
        }

        internal async Task<ViewMyEventRegistration> GetEventRegistrationDetails(string groupSlug, string eventSlug)
        {
            var client = await Client();
            return await client.GetAsJson<ViewMyEventRegistration>($"api/user/groups/{groupSlug}/events/{eventSlug}");
        }

        public async Task<ListEventAttendees[]> ListEventAttendees(string groupSlug, string eventSlug)
        {
            var client = await Client();
            return await client.GetAsJson<ListEventAttendees[]>($"/api/Groups/{groupSlug}/Events/{eventSlug}/Members");
        }
        private async Task<HttpClient> Client(bool tokenRequired = true)
        {
            if (tokenRequired)
            {
                var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("Auth0", "access_token");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return _client;
        }
    }
}
