using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to delete unused groups", SoThat = "The database can be kept tidy")]
    public class DeleteGroup_NoPermission : BaseTest
    {
        private string _token;
        private Group _existingGroup;

        public DeleteGroup_NoPermission(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmNotASystemAdministrator()
        {
            _token = await Fixture.GetTokenForNormalUser();
        }

        private async Task GivenAGroupExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/{_existingGroup.Slug}");
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }

        private async Task ThenTheGroupIsNotRemovedFromTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Slug == _existingGroup.Slug).SingleOrDefaultAsync();

            Assert.NotNull(actualGroup);
        }
    }
}
