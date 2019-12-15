using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VenimusAPIs.Tests.Infrastucture
{
    public class MockSlack : DelegatingHandler
    {
        public HttpRequestMessage LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("OK"),
            };

            return Task.FromResult(response);
        }
    }
}
