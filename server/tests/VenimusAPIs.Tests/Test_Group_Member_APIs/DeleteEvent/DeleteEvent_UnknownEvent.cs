using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to delete an existing event", SoThat = "People are kept informed")]
    public class DeleteEvent_UnknownEvent : BaseTest
    {
        private Group _group;

        public DeleteEvent_UnknownEvent(Fixture fixture) : base(fixture)
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

        private async Task GivenIAmAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_group, User);

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task WhenICallTheDeleteEventApi()
        {
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/{_group.Slug}/Events/MADEUP");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }
    }
}
