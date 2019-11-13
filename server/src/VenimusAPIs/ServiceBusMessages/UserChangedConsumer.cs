using MassTransit;
using System.Threading.Tasks;

namespace VenimusAPIs.ServiceBusMessages
{
    public class UserChangedConsumer : IConsumer<UserChangedMessage>
    {
        private readonly Mongo.GroupStore _groupStore;
        private readonly Mongo.UserStore _userStore;

        public UserChangedConsumer(Mongo.GroupStore groupStore, Mongo.UserStore userStore)
        {
            _groupStore = groupStore;
            _userStore = userStore;
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
        }
    }
}
