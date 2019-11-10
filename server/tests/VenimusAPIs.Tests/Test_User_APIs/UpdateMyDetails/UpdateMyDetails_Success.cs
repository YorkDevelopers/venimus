using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateMyDetails
{
    [Story(AsA = "User", IWant = "To be able to update my profile details", SoThat = "I can ensure they are upto date")]
    public class UpdateMyDetails_Success : BaseTest
    {
        private ViewModels.UpdateMyDetails _amendedUser;

        public UpdateMyDetails_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task WhenICallTheApi()
        {
            _amendedUser = Data.Create<ViewModels.UpdateMyDetails>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _amendedUser.ProfilePictureAsBase64 = Convert.ToBase64String(logo);

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user", _amendedUser);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenMyDetailsAreUpdatedInTheDatabase()
        {
            var users = UsersCollection();
            var actualUser = await users.Find(u => u.Id == User.Id).SingleAsync();

            Assert.Equal(_amendedUser.Bio, actualUser.Bio);
            Assert.Equal(_amendedUser.Pronoun, actualUser.Pronoun);
            Assert.Equal(_amendedUser.DisplayName, actualUser.DisplayName);
            Assert.Equal(_amendedUser.Fullname, actualUser.Fullname);
            Assert.Equal(_amendedUser.ProfilePictureAsBase64, Convert.ToBase64String(actualUser.ProfilePicture));
        }
    }
}
