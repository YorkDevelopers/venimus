using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to schedule a new event", SoThat = "People can meet up")]
    public class CreateEvent_NoPermission : BaseTest
    {
        private ViewModels.CreateEvent _event;
        private Group _group;

        public CreateEvent_NoPermission(Fixture fixture) : base(fixture)
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

        private async Task GivenAGroupExistsButIAmNotTheAdministrator()
        {
            _group = Data.Create<Models.Group>();

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task WhenICallTheCreateEventApi()
        {
            _event = Data.Create<ViewModels.CreateEvent>();

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/events", _event);
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }

        private async Task AndTheEventIsNotAddedToTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Slug == _event.Slug).SingleOrDefaultAsync();

            Assert.Null(actualEvent);
        }
    }
}
