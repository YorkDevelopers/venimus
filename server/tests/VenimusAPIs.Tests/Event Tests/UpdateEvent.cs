using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to update the details of an existing event", SoThat = "People are kept informed")]
    public class UpdateEvent : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private Event _event;
        private Group _group;
        private ViewModels.UpdateEvent _amendedEvent;

        public UpdateEvent(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAGroupAdministrator()
        {
            _token = await Fixture.GetTokenForSystemAdministrator();
        }

        private async Task GivenAGroupExists()
        {
            _group = Data.Create<Models.Group>();

            var collection = GroupsCollection();
            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExists()
        {
            _event = Data.Create<Models.Event>();

            var events = EventsCollection();

            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheUpateEventApi()
        {
            _amendedEvent = Data.Create<ViewModels.UpdateEvent>();

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_group.Name}/Events/{_event.Slug}", _amendedEvent);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        private async Task ThenTheEventIsUpdatedInTheDatabase()
        {
            var events = EventsCollection();
            var actualGroup = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();
            
            Assert.Equal(_amendedEvent.Slug, actualGroup.Slug);
            Assert.Equal(_amendedEvent.Title, actualGroup.Title);
            Assert.Equal(_amendedEvent.Description, actualGroup.Description);
            Assert.Equal(_amendedEvent.StartTime, actualGroup.StartTime);
            Assert.Equal(_amendedEvent.EndTime, actualGroup.EndTime);
            Assert.Equal(_amendedEvent.Location, actualGroup.Location);
        }
    }
}
