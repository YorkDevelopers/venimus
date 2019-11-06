using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewGroupMembers
{
    [Story(AsA = "User", IWant = "to be able to view the other members of a group", SoThat = "I can belong to the community")]
    public class ViewGroupMembers_NoPermission : BaseTest
    {
        private string _token;
        private Group _existingGroup;

        public ViewGroupMembers_NoPermission(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIAmUser()
        {
            var uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(uniqueID);
        }

        private async Task GivenIGroupIDoNotBelongTo()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApiForAnUnknownGroup()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_existingGroup.Slug}/Members");
        }

        private void ThenForbiddenIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }
    }
}