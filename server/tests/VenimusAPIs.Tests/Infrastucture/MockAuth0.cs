using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using VenimusAPIs.Services.Auth0Models;

namespace VenimusAPIs.Tests.Infrastucture
{
    public class MockAuth0 : DelegatingHandler
    {
        public UserProfile UserProfile { get; set; } = new UserProfile();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ObjectContent<UserProfile>(UserProfile, new JsonMediaTypeFormatter()),
            };

            return Task.FromResult(response);
        }
    }
}
