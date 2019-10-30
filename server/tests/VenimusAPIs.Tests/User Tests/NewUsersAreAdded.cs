﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "A new user", IWant = "To be added to the system", SoThat = "I can join groups and attend events")]
    public class NewUsersAreAdded : BaseTest
    {
        private string _uniqueID;
        private string _token;
        private string _expectedEmailAddress;

        private HttpResponseMessage _response;

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
            _expectedEmailAddress = Guid.NewGuid().ToString();
            Fixture.MockAuth0.EmailAddress = _expectedEmailAddress;
        }

        private async Task WhenICallTheUserLoggedInAPI()
        {
            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PostAsync("api/Users/Connected");

            _response.EnsureSuccessStatusCode();
        }

        private async Task ThenIAmAddedToTheDatabase()
        {
            var users = UsersCollection();

            var filter = Builders<Models.User>.Filter.AnyEq(x => x.Identities, _uniqueID);
            var actualUser = await users.Find(filter).SingleAsync();

            Assert.Equal(_expectedEmailAddress, actualUser.EmailAddress);
        }
    }
}
