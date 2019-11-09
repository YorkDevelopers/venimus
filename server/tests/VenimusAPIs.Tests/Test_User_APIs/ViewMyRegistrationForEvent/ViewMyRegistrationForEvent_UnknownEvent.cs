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
    public class ViewMyRegistrationForEvent_UnknownEvent : BaseTest
    {
        private string _token;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;

        public ViewMyRegistrationForEvent_UnknownEvent(Fixture fixture) : base(fixture)
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

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, _user);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApiForAnUnknownEvent()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/user/groups/{_existingGroup.Slug}/Events/MADEUP");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
