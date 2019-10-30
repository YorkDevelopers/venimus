using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to create new groups", SoThat = "People can build communities")]
    public class CreateGroup : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private ViewModels.CreateGroup _group;

        public CreateGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmASystemAdministrator()
        {
            _token = await Fixture.GetToken();
        }

        private async Task WhenICallTheCreateGroupApi()
        {
            _group = Data.Create<ViewModels.CreateGroup>();

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PostAsJsonAsync("api/Groups", _group);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, _response.StatusCode);
        }

        private async Task ThenANewGroupIsAddedToTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Name == _group.Name).SingleOrDefaultAsync();

            Assert.Equal(_group.Description, actualGroup.Description);
        }

        private void ThenTheLocationOfTheNewGroupIsReturned()
        {
            var location = _response.Headers.Location;
            var actualGroupName = location.Segments.Last();

            Assert.Equal(_group.Name, actualGroupName);
        }
    }
}
