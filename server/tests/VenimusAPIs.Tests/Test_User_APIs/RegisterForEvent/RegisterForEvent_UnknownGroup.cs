using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.RegisterForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class RegisterForEvent_UnknownGroup : BaseTest
    {
        private string _token;
        private string _uniqueID;
        private User _user;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_UnknownGroup(Fixture fixture) : base(fixture)
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
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PostAsJsonAsync($"api/user/groups/MADEUP/Events", _signUpToEvent);
        }

        private void ThenNotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
