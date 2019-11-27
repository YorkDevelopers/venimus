using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VenimusAPIs.Tests.Infrastucture
{
    public class MockImageSource : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3, 4 }),
            };

            return Task.FromResult(response);
        }
    }
}
