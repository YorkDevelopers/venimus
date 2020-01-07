using MassTransit;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace VenimusAPIs.ServiceBus
{
    public class GroupChangedConsumer : IConsumer<GroupChangedMessage>
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.GroupStore _groupStore;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;

        public GroupChangedConsumer(Mongo.EventStore eventStore, Mongo.GroupStore groupStore, IStringLocalizer<ResourceMessages> stringLocalizer)
        {
            _eventStore = eventStore;
            _groupStore = groupStore;
            _stringLocalizer = stringLocalizer;
        }

        public async Task Consume(ConsumeContext<GroupChangedMessage> context)
        {
            var groupID = context.Message.GroupId;
            var group = await _groupStore.RetrieveGroupByGroupId(new MongoDB.Bson.ObjectId(groupID)).ConfigureAwait(false);
            if (group == null)
            {
                throw new System.Exception(_stringLocalizer.GetString(Resources.ResourceMessages.INTERNALERROR_GROUP_DOES_NOT_EXIST).Value);
            }

            await _eventStore.UpdateGroupDetailsInEvents(group).ConfigureAwait(false);
        }
    }
}
