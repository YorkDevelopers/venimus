using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewMyRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class ViewMyRegistrationForEvent_UnknownGroup : BaseTest
    {
        private string _token;
        private string _uniqueID;
        private User _user;

        public ViewMyRegistrationForEvent_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);

            _user = Data.Create<Models.User>();

            var collection = UsersCollection();
            _user.Identities = new List<string> { _uniqueID };
            await collection.InsertOneAsync(_user);
        }

        private async Task WhenICallTheApiForAnUnknownGroup()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/user/groups/MADEUP/Events/MADEUP");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
