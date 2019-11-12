using MassTransit;
using System.Threading.Tasks;

namespace VenimusAPIs.ServiceBusMessages
{
    public class UserChangedConsumer : IConsumer<UserChangedMessage>
    {
        private readonly Services.Mongo _mongo;

        public UserChangedConsumer(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        public async Task Consume(ConsumeContext<UserChangedMessage> context)
        {
            var userID = new MongoDB.Bson.ObjectId(context.Message.UserId);

            var user = await _mongo.GetUserById(userID);
            if (user == null)
            {
                throw new System.Exception("Unknown user");
            }

            await _mongo.UpdateUserDetailsInGroups(user);
        }
    }
}
