using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeclineEvent
{
    [Story(AsA = "User", IWant = "To be able to decline events", SoThat = "The host knows I cannot attend")]
    public class DeclineEvent_UnknownEvent : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;

        public DeclineEvent_UnknownEvent(Fixture fixture) : base(fixture)
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
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            _existingGroup.Members = new List<MongoDB.Bson.ObjectId> { _user.Id };

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.DeleteAsync($"api/user/groups/{_existingGroup.Slug}/Events/MADEUP");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, _response.StatusCode);
        }
    }
}
