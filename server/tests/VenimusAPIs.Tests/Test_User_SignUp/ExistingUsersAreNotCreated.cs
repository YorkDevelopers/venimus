using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_SignUp
{
    [Story(AsA = "As a user", IWant = "To be added to the system", SoThat = "I can join groups and attend events")]
    public class ExistingUsersAreNotCreated : BaseTest
    {
        private string _uniqueID;
        private string _token;
        private string _expectedEmailAddress;

        public ExistingUsersAreNotCreated(Fixture fixture) : base(fixture)
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
            _expectedEmailAddress = Guid.NewGuid().ToString();
            Fixture.MockAuth0.UserProfile.Email = _expectedEmailAddress;
        }

        private async Task GivenIAlreadyExistInTheDatabase()
        {
            var user = Data.Create<Models.User>();

            var collection = UsersCollection();

            user.Identities = new List<string> { _uniqueID };
            await collection.InsertOneAsync(user);
        }

        private async Task WhenICallTheUserLoggedInAPI()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PostAsync("api/Users/Connected");

            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenIAmNotAddedToTheDatabase()
        {
            var users = UsersCollection();
            var filter = Builders<Models.User>.Filter.AnyEq(x => x.Identities, _uniqueID);
            var numberOfEntries = await users.Find(filter).CountDocumentsAsync();

            Assert.Equal(1, numberOfEntries);
        }

        private void ThenTheHeaderIsSetToShowTheUserWasNotCreated()
        {
            var value = bool.Parse(Response.Headers.GetValues("NewUser").First());
            Assert.False(value);
        }
        
        private void ThenThePathToUserIsReturned()
        {
            Assert.Equal($"http://localhost/api/user", Response.Headers.Location.ToString());
        }
    }
}
