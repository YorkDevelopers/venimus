using System.Net.Http;
using System.Threading.Tasks;
using VenimusAPIs.Extensions;
using VenimusAPIs.Services.SlackModels;

namespace VenimusAPIs.Services
{
    public class Slack
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Slack(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendAdvancedMessage(AdvancedMessage message, System.Uri sendTo)
        {
            var client = _httpClientFactory.CreateClient("Slack");
            var response = await client.PostAsJsonAsync(sendTo, message).ConfigureAwait(false);
            var txt = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}
