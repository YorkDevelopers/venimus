﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_SignUp
{
    [Story(AsA = "As a user", IWant = "To be able to login with different social media accounts", SoThat = "I can join groups and attend events")]
    public class UsersCanBeMerged : BaseTest
    {
        private string _newUniqueID;
        private string _originalUniqueID;
        private string _token;
        private string _expectedEmailAddress;

        public UsersCanBeMerged(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIHaveLoggedInViaAuth0()
        {
            _newUniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_newUniqueID);
        }

        private void GivenIExistInAuth0()
        {
            _expectedEmailAddress = Guid.NewGuid().ToString();
            Fixture.MockAuth0.UserProfile.Email = _expectedEmailAddress;
        }

        private async Task GivenIAlreadyExistInTheDatabaseWithTheSameEmailAddressButDifferentID()
        {
            var user = Data.Create<Models.User>();
            _originalUniqueID = Guid.NewGuid().ToString();

            var users = UsersCollection();

            user.Identities = new List<string> { _originalUniqueID };
            user.EmailAddress = _expectedEmailAddress;

            await users.InsertOneAsync(user);
        }

        private async Task WhenICallTheUserLoggedInAPI()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PostAsync("api/user/connected");

            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenIAmMergedInTheDatabase()
        {
            var users = UsersCollection();
            var actualUser = await users.Find(u => u.EmailAddress == _expectedEmailAddress).SingleAsync();

            Assert.Equal(2, actualUser.Identities.Count());
            Assert.Contains(actualUser.Identities, p => p == _newUniqueID);
            Assert.Contains(actualUser.Identities, p => p == _originalUniqueID);
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
