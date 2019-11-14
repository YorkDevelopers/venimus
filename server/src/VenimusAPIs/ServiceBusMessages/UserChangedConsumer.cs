using MassTransit;
using System.Threading.Tasks;

namespace VenimusAPIs.ServiceBusMessages
{
    public class UserChangedConsumer : IConsumer<UserChangedMessage>
    {
        private readonly Mongo.GroupStore _groupStore;
        private readonly Mongo.UserStore _userStore;
        private readonly Mongo.EventStore _eventStore;

        public UserChangedConsumer(Mongo.GroupStore groupStore, Mongo.UserStore userStore, Mongo.EventStore eventStore)
        {
            _groupStore = groupStore;
            _userStore = userStore;
            _eventStore = eventStore;
        }

        public async Task Consume(ConsumeContext<UserChangedMessage> context)
        {
            var userID = new MongoDB.Bson.ObjectId(context.Message.UserId);

            var user = await _userStore.GetUserById(userID);
            if (user == null)
            {
                throw new System.Exception("Unknown user");
            }

            await _groupStore.UpdateUserDetailsInGroups(user);

            await _eventStore.UpdateUserDetailsInEvents(user);
        }
    }
}
