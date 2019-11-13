using MassTransit;
using System.Threading.Tasks;

namespace VenimusAPIs.ServiceBusMessages
{
    public class GroupChangedConsumer : IConsumer<GroupChangedMessage>
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.GroupStore _groupStore;

        public GroupChangedConsumer(Mongo.EventStore eventStore, Mongo.GroupStore groupStore)
        {
            _eventStore = eventStore;
            _groupStore = groupStore;
        }

        public async Task Consume(ConsumeContext<GroupChangedMessage> context)
        {
            var groupID = context.Message.GroupId;
            var group = await _groupStore.RetrieveGroupByGroupId(new MongoDB.Bson.ObjectId(groupID));
            if (group == null)
            {
                throw new System.Exception("Unknown group");
            }

            await _eventStore.UpdateGroupDetailsInEvents(group);
        }
    }
}
