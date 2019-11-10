using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.LeaveGroup
{
    [Story(AsA = "User", IWant = "To be able to leave groups that I'm a member of", SoThat = "I can leave the community")]
    public class LeaveGroup_UnknownGroup : BaseTest
    {
        public LeaveGroup_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task WhenICallTheApiForAnUnknownGroup()
        {
            Response = await Fixture.APIClient.DeleteAsync($"api/User/Groups/MADEUP");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
