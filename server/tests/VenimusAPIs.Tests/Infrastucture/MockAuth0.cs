using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace VenimusAPIs.Tests.Infrastucture
{
    public class MockAuth0 : DelegatingHandler
    {
        public string EmailAddress { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var userProfile = new UserProfile
            {
                Email = EmailAddress,
            };

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ObjectContent<UserProfile>(userProfile, new JsonMediaTypeFormatter()),
            };

            return Task.FromResult(response);
        }

        private class UserProfile
        {
            [System.Text.Json.Serialization.JsonPropertyName("email")]
            [Newtonsoft.Json.JsonProperty(PropertyName ="email")]
            public string Email { get; set; }
        }
    }
}
