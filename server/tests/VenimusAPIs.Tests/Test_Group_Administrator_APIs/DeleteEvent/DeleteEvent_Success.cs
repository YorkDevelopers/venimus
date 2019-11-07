using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to delete an existing event", SoThat = "People are kept informed")]
    public class DeleteEvent_Success : BaseTest
    {
        private string _uniqueID;
        private string _token;
        private Event _event;
        private Group _group;
        private User _user;

        public DeleteEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = await Fixture.GetTokenForNormalUser(_uniqueID);

            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task GivenIAmAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_group, _user);

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExistsForTheGroup()
        {
            _event = Data.CreateEvent(_group);

            var events = EventsCollection();

            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheDeleteEventApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenTheEventIsRemoveFromTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.Null(actualEvent);
        }
    }
}
