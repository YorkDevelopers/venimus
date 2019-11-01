using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to schedule a new event", SoThat = "People can meet up")]
    public class CreateEvent : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private ViewModels.CreateEvent _event;
        private Group _group;
        private string _actualEventID;

        public CreateEvent(Fixture fixture) : base(fixture)
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

        private async Task WhenICallTheCreateEventApi()
        {
            _event = Data.Create<ViewModels.CreateEvent>();

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Name}/Events", _event);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, _response.StatusCode);
        }

        private void ThenTheLocationOfTheNewEventIsReturned()
        {
            var location = _response.Headers.Location;
            _actualEventID = location.Segments.Last();
        }

        private async Task AndANewEventIsAddedToTheDatabase()
        {
            var events = EventsCollection();
            var actualGroup = await events.Find(u => u.Id == ObjectId.Parse(_actualEventID)).SingleOrDefaultAsync();

            Assert.Equal(_event.Title, actualGroup.Title);
            Assert.Equal(_event.Description, actualGroup.Description);
            Assert.Equal(_event.StartTime, actualGroup.StartTime);
            Assert.Equal(_event.EndTime, actualGroup.EndTime);
            Assert.Equal(_event.Location, actualGroup.Location);
        }
    }
}
