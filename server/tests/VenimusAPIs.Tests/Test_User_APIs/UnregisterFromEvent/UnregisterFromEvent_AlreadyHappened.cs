using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UnregisterFromEvent
{
    [Story(AsA = "User", IWant = "To be able to decline events", SoThat = "The host knows I cannot attend")]
    public class UnregisterFromEvent_AlreadyHappened : BaseTest
    {
        private Group _existingGroup;
        private Event _existingEvent;

        public UnregisterFromEvent_AlreadyHappened(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, User);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsWhichHasAlreadyTakenPlace()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                evt.EndTimeUTC = DateTime.UtcNow.AddDays(-10);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.DeleteAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}");
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequestDetail("This event has already taken place");
        }

        private async Task ThenTheUserIsNotRecordedAsNotGoingToTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Empty(actualEvent.Members);
        }
    }
}
