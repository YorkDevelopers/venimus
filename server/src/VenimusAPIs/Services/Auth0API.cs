using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using VenimusAPIs.Services.Auth0Models;

namespace VenimusAPIs.Services
{
    public class Auth0API
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Auth0API(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<UserProfile> UserInfo(string accessToken)
        {
            var client = _httpClientFactory.CreateClient("Auth0");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

#pragma warning disable CA2234 // Pass system uri objects instead of strings
            var json = await client.GetStringAsync("/userinfo").ConfigureAwait(false);
#pragma warning restore CA2234 // Pass system uri objects instead of strings

            var profile = JsonSerializer.Deserialize<UserProfile>(json);

            return profile;
        }
    }
}
