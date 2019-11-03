using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to update some of the details of an existing event", SoThat = "People are kept informed")]
    public class PartiallyUpdateEvent : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private Event _event;
        private Group _group;
        private ViewModels.PartiallyUpdateEvent _amendedEvent;

        public PartiallyUpdateEvent(Fixture fixture) : base(fixture)
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
            _event = Data.CreateEvent(_group);

            var collection = EventsCollection();

            await collection.InsertOneAsync(_event);
        }

        private async Task WhenICallTheUpdateEventApi()
        {
            _amendedEvent = Data.Create<ViewModels.PartiallyUpdateEvent>();
            _amendedEvent.Description = null;
            _amendedEvent.EndTimeUTC = null;

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PatchAsJsonAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}", _amendedEvent);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        private async Task ThenTheEventIsUpdatedInTheDatabase()
        {
            var events = EventsCollection();
            var actualGroup = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.Equal(_amendedEvent.Slug ?? _event.Slug, actualGroup.Slug);
            Assert.Equal(_amendedEvent.Title ?? _event.Title, actualGroup.Title);
            Assert.Equal(_amendedEvent.Description ?? _event.Description, actualGroup.Description);
            Assert.Equal(_amendedEvent.StartTimeUTC ?? _event.StartTimeUTC, actualGroup.StartTimeUTC);
            Assert.Equal(_amendedEvent.EndTimeUTC ?? _event.EndTimeUTC, actualGroup.EndTimeUTC);
            Assert.Equal(_amendedEvent.Location ?? _event.Location, actualGroup.Location);
        }
    }
}
