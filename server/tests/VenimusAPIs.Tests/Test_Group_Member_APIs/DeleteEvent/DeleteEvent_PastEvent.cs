using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to delete an existing event", SoThat = "People are kept informed")]
    public class DeleteEvent_PastEvent : BaseTest
    {
        private Event _event;
        private Group _group;

        public DeleteEvent_PastEvent(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmANormalUser()
        {
            await IAmANormalUser();
        }

        private async Task GivenIAmAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_group, User);

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExistsForTheGroupWhichHasAlreadyFinished()
        {
            _event = Data.CreateEvent(_group, e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(-2);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(-1);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheDeleteEventApi()
        {
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}");
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequestDetail("The event cannot be deleted as it has already taken place.");
        }

        private async Task ThenTheEventIsNotRemovedFromTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.NotNull(actualEvent);
        }
    }
}
