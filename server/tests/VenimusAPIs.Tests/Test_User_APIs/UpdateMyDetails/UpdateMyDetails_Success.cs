using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateMyDetails
{
    [Story(AsA = "User", IWant = "To be able to update my profile details", SoThat = "I can ensure they are upto date")]
    public class UpdateMyDetails_Success : BaseTest
    {
        private string _token;
        private string _uniqueID;
        private User _user;
        private ViewModels.UpdateMyDetails _amendedUser;

        public UpdateMyDetails_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIAmUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);
        }

        private async Task GivenAlreadyExistInTheDatabase()
        {
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task WhenICallTheApi()
        {
            _amendedUser = Data.Create<ViewModels.UpdateMyDetails>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _amendedUser.ProfilePictureAsBase64 = Convert.ToBase64String(logo);

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user", _amendedUser);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenMyDetailsAreUpdatedInTheDatabase()
        {
            var users = UsersCollection();
            var actualUser = await users.Find(u => u.Id == _user.Id).SingleAsync();

            Assert.Equal(_amendedUser.Bio, actualUser.Bio);
            Assert.Equal(_amendedUser.Pronoun, actualUser.Pronoun);
            Assert.Equal(_amendedUser.DisplayName, actualUser.DisplayName);
            Assert.Equal(_amendedUser.Fullname, actualUser.Fullname);
            Assert.Equal(_amendedUser.ProfilePictureAsBase64, Convert.ToBase64String(actualUser.ProfilePicture));
        }
    }
}
