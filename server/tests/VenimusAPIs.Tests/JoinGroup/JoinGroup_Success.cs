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
    [Story(AsA = "User", IWant = "To be able to join existing groups", SoThat = "I can join the community")]
    public class JoinGroup_Success : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private ViewModels.JoinGroup _group;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;

        public JoinGroup_Success(Fixture fixture) : base(fixture)
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

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAlreadyExistInTheDatabase()
        {
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task WhenICallTheApi()
        {
            _group = Data.Create<ViewModels.JoinGroup>();
            _group.GroupSlug = _existingGroup.Slug;

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PostAsJsonAsync($"api/User/Groups", _group);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, _response.StatusCode);
        }

        private void ThenThePathToGroupIsReturned()
        {
            Assert.Equal($"http://localhost/api/User/Groups/{_existingGroup.Slug}", _response.Headers.Location.ToString());
        }

        private async Task ThenTheUserIsNowAMemberOfTheGroup()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Single(actualGroup.Members);
            Assert.Equal(_user.Id.ToString(), actualGroup.Members[0].ToString());
        }
    }
}
