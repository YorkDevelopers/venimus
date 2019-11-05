using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "User", IWant = "To be able to see what groups I'm a member of", SoThat = "I can belong to the communities")]
    public class ListMyGroups_Success : BaseTest
    {
        private string _token;
        private Group _inGroup1;
        private Group _inGroup2;
        private Group _groupNotActive;
        private Group _notInGroup;
        private string _uniqueID;
        private User _user;

        public ListMyGroups_Success(Fixture fixture) : base(fixture)
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

        private async Task GivenGroupsToWhichIBelongToExists()
        {
            _inGroup1 = Data.Create<Models.Group>(g =>
            {
                g.Members = new List<MongoDB.Bson.ObjectId>() { _user.Id };
                g.IsActive = true;
            });

            _groupNotActive = Data.Create<Models.Group>(g =>
            {
                g.Members = new List<MongoDB.Bson.ObjectId>() { _user.Id };
                g.IsActive = false;
            });

            _notInGroup = Data.Create<Models.Group>(g =>
            {
                g.IsActive = true;
            });

            _inGroup2 = Data.Create<Models.Group>(g =>
            {
                g.Members = new List<MongoDB.Bson.ObjectId>() { _user.Id };
                g.IsActive = true;
            });

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_inGroup1);
            await groups.InsertOneAsync(_groupNotActive);
            await groups.InsertOneAsync(_notInGroup);
            await groups.InsertOneAsync(_inGroup2);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/User/Groups");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheListOfActiveGroupsTheUserBelongsToAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var groups = JsonSerializer.Deserialize<ListMyGroups[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(2, groups.Length);
            AssertGroup(groups, _inGroup1);
            AssertGroup(groups, _inGroup2);
        }

        private void AssertGroup(ListMyGroups[] actualGroups, Group expectedGroup)
        {
            var actualGroup = actualGroups.Single(e => e.Slug == expectedGroup.Slug);

            Assert.Equal(expectedGroup.Slug, actualGroup.Slug);
            Assert.Equal(expectedGroup.Name, actualGroup.Name);
            Assert.Equal(expectedGroup.Description, actualGroup.Description);
            Assert.Equal(expectedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(expectedGroup.Logo, Convert.FromBase64String(actualGroup.LogoInBase64));
        }
    }
}
