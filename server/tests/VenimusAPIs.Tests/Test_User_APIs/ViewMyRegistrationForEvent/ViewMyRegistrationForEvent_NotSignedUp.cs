using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewMyRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class ViewMyRegistrationForEvent_NotSignedUp : BaseTest
    {
        private Group _existingGroup;
        private Event _existingEvent;

        public ViewMyRegistrationForEvent_NotSignedUp(Fixture fixture) : base(fixture)
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

        private async Task GivenAnEventExistsForThatGroupAndIAmNotGoing()
        {
            _existingEvent = Data.CreateEvent(_existingGroup);

            var events = EventsCollection();
            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
