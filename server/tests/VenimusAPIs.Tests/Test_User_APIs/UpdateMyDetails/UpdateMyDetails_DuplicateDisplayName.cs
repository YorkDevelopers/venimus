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
    public class UpdateMyDetails_DuplicateDisplayName : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty;
        private ViewModels.UpdateMyDetails _amendedUser;
        private User _otherUser;

        public UpdateMyDetails_DuplicateDisplayName(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "A user with this display name already exists." },
                { Cultures.Test, "'€'A user with this display name already exists." },
            }).BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

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

            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user", _amendedUser);
        }

        private async Task ThenAnErrorResponseIsReturned()
        {
            await AssertBadRequest("DisplayName", ExpectedMessage);
        }

        private async Task ThenMyDetailsAreNotUpdatedInTheDatabase()
        {
            var users = UsersCollection();
            var actualUser = await users.Find(u => u.Id == User.Id).SingleAsync();

            Assert.Equal(User.Bio, actualUser.Bio);
            Assert.Equal(User.Pronoun, actualUser.Pronoun);
            Assert.Equal(User.DisplayName, actualUser.DisplayName);
            Assert.Equal(User.Fullname, actualUser.Fullname);
            Assert.Equal(User.ProfilePicture, actualUser.ProfilePicture);
        }
    }
}
