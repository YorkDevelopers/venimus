using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewGroupMembers
{
    [Story(AsA = "User", IWant = "to be able to view the other members of a group", SoThat = "I can belong to the community")]
    public class ViewGroupMembers_UnknownGroup : BaseTest
    {
        private string _token;

        public ViewGroupMembers_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmUser()
        {
            var uniqueID = Guid.NewGuid().ToString();
            _token = await Fixture.GetTokenForNormalUser(uniqueID);
        }

        private async Task WhenICallTheApiForAnUnknownGroup()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/Groups/MADEUP/Members");
        }

        private void ThenNotFoundIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}