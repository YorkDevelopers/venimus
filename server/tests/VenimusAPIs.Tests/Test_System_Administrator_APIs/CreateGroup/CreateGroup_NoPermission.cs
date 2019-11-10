using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to create new groups", SoThat = "People can build communities")]
    public class CreateGroup_NoPermission : BaseTest
    {
        private ViewModels.CreateGroup _group;

        public CreateGroup_NoPermission(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmNotASystemAdministrator() => IAmANormalUser();

        private async Task WhenICallTheCreateGroupApi()
        {
            _group = Data.Create<ViewModels.CreateGroup>();

            Response = await Fixture.APIClient.PostAsJsonAsync("api/Groups", _group);
        }

        private void ThenAForbiddenResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }

        private async Task ThenTheGroupIsNotAddedToTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroups = await groups.Find(u => u.Slug == _group.Slug).SingleOrDefaultAsync();
            Assert.Null(actualGroups);
        }
    }
}
