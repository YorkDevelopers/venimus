using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewSingleGroupMembership
{
    [Trait("API", "GET api/User/Groups/{_theGroup.Slug}")]
    [Story(AsA = "User", IWant = "To be able to see the details of a single group I'm a member of", SoThat = "I can belong to the community")]
    public class ViewSingleGroupMembership_NotAMember : BaseTest
    {
        private string _token;
        private Group _theGroup;
        private string _uniqueID;
        private User _user;

        public ViewSingleGroupMembership_NotAMember(Fixture fixture) : base(fixture)
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

        private async Task GivenIDotNotBelongToTheGroup()
        {
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");

            _theGroup = Data.Create<Models.Group>(g =>
            {
                g.IsActive = true;
                g.Logo = logo;
            });

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_theGroup);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/User/Groups/{_theGroup.Slug}");
        }

        private void ThenA404ResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
