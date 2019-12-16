using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using VenimusAPIs.Services.SlackModels;

namespace VenimusAPIs.Services
{
    public class SlackMessages
    {
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;
        private readonly URLBuilder _urlBuilder;

        public SlackMessages(
                     IStringLocalizer<ResourceMessages> stringLocalizer,
                     URLBuilder urlBuilder)
        {
            _stringLocalizer = stringLocalizer;
            _urlBuilder = urlBuilder;
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
                             ImageURL = new Uri(_urlBuilder.BuildUserDetailsProfilePictureURL(user)),
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
                                ActionID = SlackActionTypes.Approve,
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
                                ActionID = SlackActionTypes.Reject,
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

        public AdvancedMessage BuildApprovedRequestMessage(Models.User user)
        {
            var advancedMessage = new AdvancedMessage
            {
                ReplaceOriginal = true,
                Blocks = new List<IBlock>()
                {
                    new SectionBlock
                    {
                         Type = "section",
                         Text = new MarkdownText
                         {
                             Text = $"The request for {user.Fullname} to join YorkDevelopers as been approved by {user.ApprovedorRejectedBy}.",
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
                             ImageURL = new Uri(_urlBuilder.BuildUserDetailsProfilePictureURL(user)),
                             AltText = "Profile Picture",
                         },
                    },
                },
            };

            return advancedMessage;
        }

        public AdvancedMessage BuildRejectedRequestMessage(Models.User user)
        {
            var advancedMessage = new AdvancedMessage
            {
                ReplaceOriginal = true,
                Blocks = new List<IBlock>()
                {
                    new SectionBlock
                    {
                         Type = "section",
                         Text = new MarkdownText
                         {
                             Text = $"The request for {user.Fullname} to join YorkDevelopers as been rejected by {user.ApprovedorRejectedBy}.",
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
                             ImageURL = new Uri(_urlBuilder.BuildUserDetailsProfilePictureURL(user)),
                             AltText = "Profile Picture",
                         },
                    },
                },
            };

            return advancedMessage;
        }
    }
}
