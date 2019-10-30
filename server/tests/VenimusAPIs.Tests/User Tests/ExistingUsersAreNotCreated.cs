using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "As a user", IWant = "To be added to the system", SoThat = "I can join groups and attend events")]
    public class ExistingUsersAreNotCreated : BaseTest
    {
        private string _uniqueID;
        private string _token;
        private string _expectedEmailAddress;

        private HttpResponseMessage _response;

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
            Fixture.MockAuth0.EmailAddress = _expectedEmailAddress;
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
            _response = await Fixture.APIClient.PostAsync("api/Users/Connected");

            _response.EnsureSuccessStatusCode();
        }

        private async Task ThenIAmNotAddedToTheDatabase()
        {
            var users = UsersCollection();
            var filter = Builders<Models.User>.Filter.AnyEq(x => x.Identities, _uniqueID);
            var numberOfEntries = await users.Find(filter).CountDocumentsAsync();

            Assert.Equal(1, numberOfEntries);
        }
    }
}
