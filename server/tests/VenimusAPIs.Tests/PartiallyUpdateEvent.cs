using System.Net.Http;
using System.Threading.Tasks;
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
            _token = await Fixture.GetToken();
        }

        private async Task GivenAGroupExists()
        {
            _group = Data.Create<Models.Group>();

            var mongoDatabase = Fixture.MongoDatabase();
            var collection = mongoDatabase.GetCollection<Models.Group>("groups");

            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExists()
        {
            _event = Data.Create<Models.Event>();

            var mongoDatabase = Fixture.MongoDatabase();
            var collection = mongoDatabase.GetCollection<Models.Event>("events");

            await collection.InsertOneAsync(_event);
        }

        private async Task WhenICallTheUpateEventApi()
        {
            _amendedEvent = Data.Create<ViewModels.PartiallyUpdateEvent>();
            _amendedEvent.Description = null;
            _amendedEvent.EndTime = null;

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PatchAsJsonAsync($"api/Groups/{_group.Name}/Events/{_event.EventID}", _amendedEvent);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        private async Task ThenTheEventIsUpdatedInTheDatabase()
        {
            var mongoDatabase = Fixture.MongoDatabase();
            var events = mongoDatabase.GetCollection<Models.Event>("events");
            var actualGroup = await events.Find(u => u.EventID == _event.EventID).SingleOrDefaultAsync();

            Assert.Equal(_amendedEvent.Title ?? _event.Title, actualGroup.Title);
            Assert.Equal(_amendedEvent.Description ?? _event.Description, actualGroup.Description);
            Assert.Equal(_amendedEvent.StartTime ?? _event.StartTime, actualGroup.StartTime);
            Assert.Equal(_amendedEvent.EndTime ?? _event.EndTime, actualGroup.EndTime);
            Assert.Equal(_amendedEvent.Location ?? _event.Location, actualGroup.Location);
        }
    }
}
