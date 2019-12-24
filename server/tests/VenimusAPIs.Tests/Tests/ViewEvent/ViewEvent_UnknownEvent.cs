using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewEvent
{
    [Story(AsA = "ViewEvent", IWant = "To be able to view an existing event", SoThat = "I know the details")]
    public class ViewEvent_UnknownEvent : BaseTest
    {
        private Group _group;

        public ViewEvent_UnknownEvent(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmANormalUser() => IAmANormalUser();

        private async Task GivenIAmAMemberOfTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupMember(_group, User);

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task WhenICallTheGetEventApiForAnEventtWhichDoesNotExist()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_group.Slug}/Events/MADEUP");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
