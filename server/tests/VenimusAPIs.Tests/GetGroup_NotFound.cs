using System.Net.Http;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to retrieve existing groups", SoThat = "I can maintain them")]
    public class GetGroup_NotFound : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private Models.Group _expectedGroup;

        public GetGroup_NotFound(Fixture fixture) : base(fixture)
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

        private void GivenTheGroupDoesNotExists()
        {
            _expectedGroup = Data.Create<Models.Group>();
        }

        private async Task WhenICallTheGetGroupApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.GetAsync($"api/Groups/{_expectedGroup.Name}");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, _response.StatusCode);
        }
    }
}
