using System;
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
        private string _token;
        private Event _event;
        private Group _group;
        private User _user;

        public ViewEvent_NoPermission(Fixture fixture) : base(fixture)
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

            _user = Data.Create<Models.User>();
            var collection = UsersCollection();
            await collection.InsertOneAsync(_user);
        }

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
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}");
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }
    }
}
