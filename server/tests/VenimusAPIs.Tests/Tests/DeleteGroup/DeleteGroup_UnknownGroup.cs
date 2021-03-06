using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to delete unused groups", SoThat = "The database can be kept tidy")]
    public class DeleteGroup_UnknownGroup : BaseTest
    {
        private Group _existingGroup;

        public DeleteGroup_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmASystemAdministrator() => IAmASystemAdministrator();

        private void GivenTheGroupDoesNotExist()
        {
            _existingGroup = Data.Create<Models.Group>();
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/{_existingGroup.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }
    }
}
