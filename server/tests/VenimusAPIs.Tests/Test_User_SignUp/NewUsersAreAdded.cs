using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Services.Auth0Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_SignUp
{
    [Story(AsA = "A new user", IWant = "To be added to the system", SoThat = "I can join groups and attend events")]
    public class NewUsersAreAdded : BaseTest
    {
        private string _uniqueID;
        private string _token;
        private ObjectId _actualUserID;

        public NewUsersAreAdded(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIHaveLoggedInViaAuth0()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);
        }

        private void GivenIExistInAuth0()
        {
            Fixture.MockAuth0.UserProfile = Data.Create<UserProfile>();
        }

        private async Task WhenICallTheUserLoggedInAPI()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PostAsync("api/user/connected");

            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenIAmAddedToTheDatabase()
        {
            var users = UsersCollection();

            var filter = Builders<Models.User>.Filter.AnyEq(x => x.Identities, _uniqueID);
            var actualUser = await users.Find(filter).SingleAsync();

            Assert.Equal(Fixture.MockAuth0.UserProfile.Email, actualUser.EmailAddress);
            Assert.Equal(string.Empty, actualUser.Pronoun);
            Assert.Equal(Fixture.MockAuth0.UserProfile.Name, actualUser.Fullname);
            Assert.Equal(string.Empty, actualUser.DisplayName);
            Assert.Equal(string.Empty, actualUser.Bio);

            _actualUserID = actualUser.Id;

            // Assert.Equal(Fixture.MockAuth0.UserProfile.Picture, actualUser.ProfilePicture);
        }

        private void ThenTheHeaderIsSetToShowTheUserWasCreated()
        {
            var value = bool.Parse(Response.Headers.GetValues("NewUser").First());
            Assert.True(value);
        }

        private void ThenThePathToUserIsReturned()
        {
            Assert.Equal($"http://localhost/api/user", Response.Headers.Location.ToString());
        }

        private void AndThenTheHeaderContainsTheUrlToTheUsersProfilePicture()
        {
            var value = Response.Headers.GetValues("ProfilePictureURL").First();
            Assert.Equal($"http://localhost/api/users/{_actualUserID}/profilepicture", value);
        }
    }
}
