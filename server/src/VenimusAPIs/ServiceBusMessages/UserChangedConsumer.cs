using MassTransit;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using VenimusAPIs.Services;
using VenimusAPIs.Settings;

namespace VenimusAPIs.ServiceBusMessages
{
    public class UserChangedConsumer : IConsumer<UserChangedMessage>
    {
        private readonly Mongo.GroupStore _groupStore;
        private readonly Mongo.UserStore _userStore;
        private readonly Mongo.EventStore _eventStore;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;
        private readonly Slack _slack;
        private readonly SlackMessages _slackMessages;
        private readonly SlackSettings _slackSettings;

        public UserChangedConsumer(
                                   Mongo.GroupStore groupStore, 
                                   Mongo.UserStore userStore, 
                                   Mongo.EventStore eventStore, 
                                   IStringLocalizer<ResourceMessages> stringLocalizer,
                                   Slack slack, 
                                   SlackMessages slackMessages, 
                                   IOptions<Settings.SlackSettings> slackSettings)
        {
            _groupStore = groupStore;
            _userStore = userStore;
            _eventStore = eventStore;
            _stringLocalizer = stringLocalizer;
            _slack = slack;
            _slackMessages = slackMessages;
            _slackSettings = slackSettings.Value;
        }

        public async Task Consume(ConsumeContext<UserChangedMessage> context)
        {
            var userID = new MongoDB.Bson.ObjectId(context.Message.UserId);

            var user = await _userStore.GetUserById(userID).ConfigureAwait(false);
            if (user == null)
            {
                throw new System.Exception(_stringLocalizer.GetString(Resources.ResourceMessages.INTERNALERROR_USER_DOES_NOT_EXIST).Value);
            }

            await _groupStore.UpdateUserDetailsInGroups(user).ConfigureAwait(false);

            await _eventStore.UpdateUserDetailsInEvents(user).ConfigureAwait(false);

            if (user.IsRegistered && (!user.IsApproved) && (!user.IsRejected))
            {
                var message = _slackMessages.BuildApprovalRequestMessage(user);
                await _slack.SendAdvancedMessage(message, _slackSettings.ApproversWebhookUrl).ConfigureAwait(false);
            }
        }
    }
}
