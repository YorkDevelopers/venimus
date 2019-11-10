using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewEvent
{
    [Story(AsA = "ViewEvent", IWant = "To be able to view an existing event", SoThat = "I know the details")]
    public class ViewEvent_NoPermission : BaseTest
    {
        private Event _event;
        private Group _group;

        public ViewEvent_NoPermission(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmANormalUser() => IAmANormalUser();

        private async Task GivenIAmNotAMemberOfTheGroup()
        {
            _group = Data.Create<Models.Group>();

            var collection = GroupsCollection();
            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExistsForTheGroup()
        {
            _event = Data.CreateEvent(_group);

            var events = EventsCollection();
            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheGetEventApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}");
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }
    }
}
