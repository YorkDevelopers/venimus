using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_APIs.ViewMyDetails
{
    [Story(AsA = "User", IWant = "To be able to view a user's picture", SoThat = "I know who they are")]
    public class ViewUserProfilePicture_Success : BaseTest
    {
        public ViewUserProfilePicture_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private Task GivenIAmNormalUser() => IAmANormalUser();

        private async Task WhenICallTheGetUserProfilePictureApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Users/{User.Id}/ProfilePicture");
            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheProfilePictureIsReturned()
        {
            var actualProfilePicture = await Response.Content.ReadAsByteArrayAsync();
            Assert.Equal(User.ProfilePicture, actualProfilePicture);
        }
    }
}
