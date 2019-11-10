using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.JoinGroup
{
    [Story(AsA = "User", IWant = "To be able to join existing groups", SoThat = "I can join the community")]
    public class JoinGroup_UnknownGroup : BaseTest
    {
        public JoinGroup_UnknownGroup(Fixture fixture) : base(fixture)
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
            var group = Data.Create<ViewModels.JoinGroup>();
            group.GroupSlug = "MADEUP";

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/User/Groups", group);
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
