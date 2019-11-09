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
    public class DeleteEvent_NoPermission : BaseTest
    {
        private Event _event;
        private Group _group;

        public DeleteEvent_NoPermission(Fixture fixture) : base(fixture)
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
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}");
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }

        private async Task ThenTheEventIsNotRemovedFromTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.NotNull(actualEvent);
        }
    }
}
