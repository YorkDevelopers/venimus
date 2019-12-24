using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_APIs.GetUserDetails
{
    public class GetUserDetails_NotFound : BaseTest
    {
        public GetUserDetails_NotFound(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmANormalUser() => IAmANormalUser();

        private async Task WhenICallTheApiForAMadeUpUser()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Users?DisplayName=MADEUP");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
