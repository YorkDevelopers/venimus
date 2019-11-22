using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_APIs.ViewMyDetails
{
    [Story(AsA = "User", IWant = "To be able to view a user's picture", SoThat = "I know who they are")]
    public class ViewUserProfilePicture_UnknownUser : BaseTest
    {
        public ViewUserProfilePicture_UnknownUser(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private async Task WhenICallTheGetUserProfilePictureApiForAnUnknownUser()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Users/MADEUP/ProfilePicture");
        }

        private void ThenNotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
