using MassTransit;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace VenimusAPIs.ServiceBusMessages
{
    public class UserChangedConsumer : IConsumer<UserChangedMessage>
    {
        private readonly Mongo.GroupStore _groupStore;
        private readonly Mongo.UserStore _userStore;
        private readonly Mongo.EventStore _eventStore;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;

        public UserChangedConsumer(Mongo.GroupStore groupStore, Mongo.UserStore userStore, Mongo.EventStore eventStore, IStringLocalizer<ResourceMessages> stringLocalizer)
        {
            _groupStore = groupStore;
            _userStore = userStore;
            _eventStore = eventStore;
            _stringLocalizer = stringLocalizer;
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
        }
    }
}
