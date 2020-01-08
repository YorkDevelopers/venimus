using MassTransit;
using System.Threading.Tasks;
using VenimusAPIs.Models;

namespace VenimusAPIs.ServiceBus
{
    public class EventPublisher
    {
        private readonly IBus _bus;

        public EventPublisher(IBus bus)
        {
            _bus = bus;
        }

        public Task GroupChanged(Group group)
        {
            var groupChangedMessage = new GroupChangedMessage { GroupId = group.Id.ToString() };

            return _bus.Publish(groupChangedMessage);
        }

        public Task UserChanged(User user)
        {
            var userChangedMessage = new UserChangedMessage { UserId = user.Id.ToString() };

            return _bus.Publish(userChangedMessage);
        }
    }
}
