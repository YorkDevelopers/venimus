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

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIAmASystemAdministrator()
        {
        }

        private async Task WhenICallTheCreateGroupApi()
        {
            var newGroup = new ViewModels.CreateNewGroup
            {
            };

            APIClient.SetBearerToken("PUT TOKEN HERE");
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
