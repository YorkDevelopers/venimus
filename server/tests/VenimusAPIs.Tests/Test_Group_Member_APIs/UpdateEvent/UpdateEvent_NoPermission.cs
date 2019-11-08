using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to update the details of an existing event", SoThat = "People are kept informed")]
    public class UpdateEvent_NoPermission : BaseTest
    {
        private string _token;
        private Event _event;
        private Group _group;
        private ViewModels.UpdateEvent _amendedEvent;

        public UpdateEvent_NoPermission(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAUser()
        {
            var uniqueID = Guid.NewGuid().ToString();
            _token = await Fixture.GetTokenForNormalUser(uniqueID);

            var user = Data.Create<Models.User>();

            var collection = UsersCollection();

            user.Identities = new List<string> { uniqueID };

            await collection.InsertOneAsync(user);
        }

        private async Task GivenIAmNotAnAdminstratorForTheGroup()
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

        private async Task WhenICallTheUpdateEventApi()
        {
            _amendedEvent = Data.Create<ViewModels.UpdateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
            });

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}", _amendedEvent);
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }

        private async Task ThenTheEventIsNotUpdatedInTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.Equal(_event.Slug, actualEvent.Slug);
            Assert.Equal(_event.Title, actualEvent.Title);
            Assert.Equal(_event.Description, actualEvent.Description);
            AssertDateTime(_event.StartTimeUTC, actualEvent.StartTimeUTC);
            AssertDateTime(_event.EndTimeUTC, actualEvent.EndTimeUTC);
            Assert.Equal(_event.Location, actualEvent.Location);
            Assert.Equal(_event.GroupId, actualEvent.GroupId);
            Assert.Equal(_event.GroupName, actualEvent.GroupName);
            Assert.Equal(_event.GroupSlug, actualEvent.GroupSlug);
        }
    }
}
