using MassTransit;
using System.Threading.Tasks;

namespace VenimusAPIs.ServiceBusMessages
{
    public class GroupChangedConsumer : IConsumer<GroupChangedMessage>
    {
        private readonly Services.Mongo _mongo;

        public GroupChangedConsumer(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        public async Task Consume(ConsumeContext<GroupChangedMessage> context)
        {
            var groupID = context.Message.GroupId;
            var group = await _mongo.RetrieveGroupByGroupId(new MongoDB.Bson.ObjectId(groupID));
            if (group == null)
            {
                throw new System.Exception("Unknown group");
            }

            await _mongo.UpdateGroupDetailsInEvents(group);
        }
    }
}
