using System.Net.Http;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to create new groups", SoThat = "People can build communities")]
    public class CreateGroup : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmASystemAdministrator()
        {
            _token = await GetToken();
        }

        private async Task WhenICallTheCreateGroupApi()
        {
            var newGroup = new ViewModels.CreateNewGroup
            {
            };
            
            APIClient.SetBearerToken(_token);
            _response = await APIClient.PostAsJsonAsync("api/Group", newGroup);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        private void ThenANewGroupIsAddedToTheDatabase()
        {
            Assert.False(true);
        }

        private void ThenTheLocationOfTheNewGroupIsReturned()
        {
            Assert.False(true);
        }
    }
}
