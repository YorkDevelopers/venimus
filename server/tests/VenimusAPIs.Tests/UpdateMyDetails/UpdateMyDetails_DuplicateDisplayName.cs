using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateMyDetails
{
    [Story(AsA = "User", IWant = "To be able to update my profile details", SoThat = "I can ensure they are upto date")]
    public class UpdateMyDetails_DuplicateDisplayName : BaseTest
    {
        private string _token;
        private string _uniqueID;
        private User _originalUserDetails;
        private ViewModels.UpdateMyDetails _amendedUser;
        private User _otherUser;

        public UpdateMyDetails_DuplicateDisplayName(Fixture fixture) : base(fixture)
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
            _originalUserDetails = Data.Create<Models.User>();

            var collection = UsersCollection();

            _originalUserDetails.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_originalUserDetails);
        }

        private async Task GivenAnotherUserExistsInTheDatabase()
        {
            _otherUser = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertOneAsync(_otherUser);
        }

        private async Task WhenICallTheApiWithTheSameDisplayName()
        {
            _amendedUser = Data.Create<ViewModels.UpdateMyDetails>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _amendedUser.ProfilePictureAsBase64 = Convert.ToBase64String(logo);

            _amendedUser.DisplayName = _otherUser.DisplayName;

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user", _amendedUser);
        }

        private async Task ThenAnErrorResponseIsReturned()
        {
            await AssertBadRequest("DisplayName", "A user with this display name already exists.");
        }

        private async Task ThenMyDetailsAreNotUpdatedInTheDatabase()
        {
            var users = UsersCollection();
            var actualUser = await users.Find(u => u.Id == _originalUserDetails.Id).SingleAsync();

            Assert.Equal(_originalUserDetails.Bio, actualUser.Bio);
            Assert.Equal(_originalUserDetails.Pronoun, actualUser.Pronoun);
            Assert.Equal(_originalUserDetails.DisplayName, actualUser.DisplayName);
            Assert.Equal(_originalUserDetails.Fullname, actualUser.Fullname);
            Assert.Equal(_originalUserDetails.ProfilePicture, actualUser.ProfilePicture);
        }
    }
}
