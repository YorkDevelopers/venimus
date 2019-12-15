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
    public partial class Slack
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;
        private readonly URLBuilder _urlBuilder;
        private readonly SlackSettings _slackSettings;

        public Slack(
                     IHttpClientFactory httpClientFactory,
                     IOptions<SlackSettings> settings,
                     IStringLocalizer<ResourceMessages> stringLocalizer,
                     URLBuilder urlBuilder)
        {
            _httpClientFactory = httpClientFactory;
            _stringLocalizer = stringLocalizer;
            _urlBuilder = urlBuilder;
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

        public async Task SendAdvancedMessage(AdvancedMessage message)
        {
            var client = _httpClientFactory.CreateClient("Slack");
            var response = await client.PostAsJsonAsync(_slackSettings.ApproversWebhookUrl, message).ConfigureAwait(false);
            var txt = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public AdvancedMessage BuildApprovalRequestMessage(Models.User user)
        {
            var advancedMessage = new AdvancedMessage
            {
                Blocks = new List<IBlock>()
                {
                    new SectionBlock
                    {
                         Type = "section",
                         Text = new MarkdownText
                         {
                             Text = $"{user.Fullname} would like to join YorkDevelopers",
                         },
                    },
                    new SectionBlock
                    {
                         Type = "section",
                         Text = new MarkdownText
                         {
                             Text = $"<mailto:{user.EmailAddress}|{user.EmailAddress}> \n \n {user.Bio}",
                         },
                         Accessory = new Accessory
                         {
                             Type = "image",
                             ImageURL = _urlBuilder.BuildUserDetailsProfilePictureURL(user),
                             AltText = "Profile Picture",
                         },
                    },
                    new ActionBlock
                    {
                        Type = "actions",
                        Elements = new Element[]
                        {
                            new Element
                            {
                                Type = "button",
                                ActionID = "APPROVE_MEMBERSHIP",
                                Text = new ButtonText
                                {
                                    Type = "plain_text",
                                    Text = _stringLocalizer.GetString(Resources.ResourceMessages.SLACK_APPROVE_BUTTON).Value,
                                    Emoji = true,
                                },
                                Style = "primary",
                                Value = user.Id.ToString(),
                            },
                            new Element
                            {
                                Type = "button",
                                ActionID = "REJECT_MEMBERSHIP",
                                Text = new ButtonText
                                {
                                    Type = "plain_text",
                                    Text = _stringLocalizer.GetString(Resources.ResourceMessages.SLACK_REJECT_BUTTON).Value,
                                    Emoji = true,
                                },
                                Style = "danger",
                                Value = user.Id.ToString(),
                            },
                        },
                    },
                },
            };

            return advancedMessage;
        }
    }
}
