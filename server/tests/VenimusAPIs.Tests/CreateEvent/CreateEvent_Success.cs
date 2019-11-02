using System;
using System.Collections.Generic;
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
    public class CreateEvent_Success : BaseTest
    {
        private HttpResponseMessage _response;
        private string _uniqueID;
        private string _token;
        private ViewModels.CreateEvent _event;
        private Group _group;
        private User _user;

        public CreateEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAGroupAdministrator()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = await Fixture.GetTokenForNormalUser(_uniqueID);
        }

        private async Task GivenAlreadyExistInTheDatabase()
        {
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task GivenIAmAnAdminstratorForTheGroupExists()
        {
            _group = Data.Create<Models.Group>();
            _group.Administrators = new List<MongoDB.Bson.ObjectId> { _user.Id };

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task WhenICallTheCreateEventApi()
        {
            _event = Data.Create<ViewModels.CreateEvent>();

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/events", _event);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, _response.StatusCode);
        }

        private void ThenTheLocationOfTheNewEventIsReturned()
        {
            var location = _response.Headers.Location.ToString();
            Assert.Equal($"http://localhost/api/groups/{_group.Slug}/events/{_event.Slug}", location);
        }

        private async Task AndANewEventIsAddedToTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Slug == _event.Slug).SingleAsync();

            Assert.Equal(_event.Slug, actualEvent.Slug);
            Assert.Equal(_event.Title, actualEvent.Title);
            Assert.Equal(_event.Description, actualEvent.Description);
            Assert.Equal(_event.StartTime, actualEvent.StartTime);
            Assert.Equal(_event.EndTime, actualEvent.EndTime);
            Assert.Equal(_event.Location, actualEvent.Location);
            Assert.Equal(_group.Id, actualEvent.GroupId);
            Assert.Equal(_group.Slug, actualEvent.GroupSlug);
        }
    }
}
