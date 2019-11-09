using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to delete an existing event", SoThat = "People are kept informed")]
    public class DeleteEvent_SystemAdmin_Success : BaseTest
    {
        private Event _event;
        private Group _group;

        public DeleteEvent_SystemAdmin_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmASystemAdministrator()
        {
            await IAmASystemAdministrator();
        }

        private async Task GivenIAmNotAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupMember(_group, User);

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
            Fixture.APIClient.SetBearerToken(Token);
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
