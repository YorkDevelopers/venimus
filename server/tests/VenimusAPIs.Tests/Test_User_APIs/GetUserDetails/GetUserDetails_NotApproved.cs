using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_APIs.GetUserDetails
{
    public class GetUserDetails_NotApproved : BaseTest
    {
        public GetUserDetails_NotApproved(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }
        
        private Task GivenIAmAUser() => IAmANormalUser(isApproved: false);

        private async Task WhenICallTheApiForAnyUser()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Users?DisplayName=ANYTHING");
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }
    }
}
