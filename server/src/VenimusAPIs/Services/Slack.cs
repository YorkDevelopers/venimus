using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VenimusAPIs.Extensions;

namespace VenimusAPIs.Services
{
#pragma warning disable SA1027 // Use tabs correctly
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CA1303 // Do not pass literals as localized parameters

    public class Slack
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Slack(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendBasicMessage(string message)
        {
            var data = new
            {
                text = message,
            };

            var client = _httpClientFactory.CreateClient("Slack");
            var response = await client.PostAsJsonAsync(new Uri("XXXXXX), data).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendMessage(Models.User user)
        {
            var data = new AdvancedMessage
            {
                blocks = new List<Block>()
                {
                    new SectionBlock
                    {
                         type = "section",
                         text = new Text
                         {
                             type = "mrkdwn",
                             text = $"{user.Fullname} would like to join the group YorkCodeDojo:",
                         },
                    },
                    new SectionBlock
                    {
                         type = "section",
                         text = new Text
                         {
                             type = "mrkdwn",
                             text = $"<mailto:{user.EmailAddress}|{user.EmailAddress}> \n \n {user.Bio}",
                         },
                         accessory = new Accessory
                         {
                             type = "image",
                             image_url = "https://venimus-api.azurewebsites.net/api/users/5df4172d3f14e0a4c0c03ecd/profilepicture",
                             alt_text = "Profile Picture",
                         },
                    },
                    new ActionBlock
                    {
                        type = "actions",
                        elements = new Element[]
                        {
                            new Element
                            {
                                type = "button",
                                text = new ButtonText
                                {
                                    type = "plain_text",
                                    text = "Approve",
                                    emoji = true,
                                },
                                style = "primary",
                                value = user.Id.ToString(),
                            },
                            new Element
                            {
                                type = "button",
                                text = new ButtonText
                                {
                                    type = "plain_text",
                                    text = "Reject",
                                    emoji = true,
                                },
                                style = "danger",
                                value = user.Id.ToString(),
                            },
                        },
                    },
                },
            };

            var client = _httpClientFactory.CreateClient("Slack");
            var response = await client.PostAsJsonAsync(new Uri("XXXX"), data).ConfigureAwait(false);
            var txt = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        private class AdvancedMessage
        {
            public List<Block> blocks { get; set; }
        }

        private class Block
        {
            public string type { get; set; }
        }

        private class ActionBlock : Block
        {
            public Element[] elements { get; set; }
        }

        private class SectionBlock : Block
        {
            public Text text { get; set; }

            public Accessory accessory { get; set; }
        }

        private class Text
        {
            public string type { get; set; }

            public string text { get; set; }
        }

        private class ButtonText
        {
            public string type { get; set; }

            public string text { get; set; }

            public bool emoji { get; set; }
        }

        private class Accessory
        {
            public string type { get; set; }

            public string image_url { get; set; }

            public string alt_text { get; set; }
        }

        private class Element
        {
            public string type { get; set; }
            
            public string style { get; set; }

            public string value { get; set; }

            public ButtonText text { get; set; }
        }
    }
}
