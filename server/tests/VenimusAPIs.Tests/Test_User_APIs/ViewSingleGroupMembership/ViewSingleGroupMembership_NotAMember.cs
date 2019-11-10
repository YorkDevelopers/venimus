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
        private Group _theGroup;

        public ViewSingleGroupMembership_NotAMember(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

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
            Response = await Fixture.APIClient.GetAsync($"api/User/Groups/{_theGroup.Slug}");
        }

        private void ThenA404ResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
