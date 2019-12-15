using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VenimusAPIs.Extensions;
using VenimusAPIs.Services.SlackModels;
using VenimusAPIs.Settings;

namespace VenimusAPIs.Services
{
    public class Slack
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SlackSettings _slackSettings;

        public Slack(
                     IHttpClientFactory httpClientFactory,
                     IOptions<SlackSettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _slackSettings = settings.Value;
        }

        public async Task SendBasicMessage(string message)
        {
            var data = new
            {
                text = message,
            };

            var client = _httpClientFactory.CreateClient("Slack");
            var response = await client.PostAsJsonAsync(_slackSettings.ApproversWebhookUrl, data).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public Task SendAdvancedMessage(AdvancedMessage message) => SendAdvancedMessage(message, _slackSettings.ApproversWebhookUrl);

        public async Task SendAdvancedMessage(AdvancedMessage message, System.Uri sendTo)
        {
            var client = _httpClientFactory.CreateClient("Slack");
            var response = await client.PostAsJsonAsync(sendTo, message).ConfigureAwait(false);
            var txt = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}
