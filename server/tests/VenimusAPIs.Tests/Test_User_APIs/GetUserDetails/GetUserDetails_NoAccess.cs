using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_APIs.GetUserDetails
{
    public class GetUserDetails_NoAccess : BaseTest
    {
        public GetUserDetails_NoAccess(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task WhenICallTheApiForAnyUser()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Users?DisplayName=ANYTHING");
        }

        private void ThenAUnAuthorisedResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, Response.StatusCode);
        }
    }
}
