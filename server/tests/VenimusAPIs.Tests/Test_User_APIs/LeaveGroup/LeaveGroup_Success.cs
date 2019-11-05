using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "User", IWant = "To be able to leave groups that I'm a member of", SoThat = "I can leave the community")]
    public class LeaveGroup_Success : BaseTest
    {
        private string _token;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;

        public LeaveGroup_Success(Fixture fixture) : base(fixture)
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

        private async Task GivenAGroupWhichIBelongToExists()
        {
            _existingGroup = Data.Create<Models.Group>();
            _existingGroup.Members = new List<MongoDB.Bson.ObjectId>() { _user.Id };

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.DeleteAsync($"api/User/Groups/{_existingGroup.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenTheUserIsNoLongerAMemberOfTheGroup()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Empty(actualGroup.Members);
        }
    }
}
