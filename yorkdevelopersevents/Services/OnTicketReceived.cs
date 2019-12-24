using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YorkDeveloperEvents.Services
{
    public class OnTicketReceived
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OnTicketReceived(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        internal async Task Execute(TicketReceivedContext context)
        {
            var tokens = context.Properties.GetTokens() as List<AuthenticationToken>;
            var accessToken = tokens.Single(token => token.Name == "access_token").Value;

            var httpClient = _httpClientFactory.CreateClient("API");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.PostAsync("api/user/connected", null);
            response.EnsureSuccessStatusCode();

            var claims = new List<Claim>
            {
                new Claim("UserProfileURL", response.Headers.Location.ToString()),
                new Claim("ProfilePictureURL", response.Headers.GetValues("profilepictureurl").First()),
                new Claim("CanCreateGroups", response.Headers.GetValues("IsSystemAdministrator").First()),
                new Claim("NewUser", (response.StatusCode == System.Net.HttpStatusCode.Created).ToString()),
            };

            var appIdentity = new ClaimsIdentity(claims);
            context.Principal.AddIdentity(appIdentity);
        }
    }
}
