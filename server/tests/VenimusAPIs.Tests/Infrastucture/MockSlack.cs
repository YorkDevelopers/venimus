using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VenimusAPIs.Tests.Infrastucture
{
    public class MockSlack : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("OK"),
            };

            return Task.FromResult(response);
        }
    }
}
