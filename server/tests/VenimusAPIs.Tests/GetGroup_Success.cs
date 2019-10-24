using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to retrieve existing groups", SoThat = "I can maintain them")]
    public class GetGroup_Success : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private Models.Group _expectedGroup;

        public GetGroup_Success(Fixture fixture) : base(fixture)
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

        private async Task GivenAGroupAlreadyExists()
        {
            _expectedGroup = Data.Create<Models.Group>();

            var mongoDatabase = Fixture.MongoDatabase();
            var collection = mongoDatabase.GetCollection<Models.Group>("groups");

            await collection.InsertOneAsync(_expectedGroup);
        }

        private async Task WhenICallTheGetGroupApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.GetAsync($"api/Groups/{_expectedGroup.Name}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        private async Task ThenTheDetailsOfTheGroupAreReturned()
        {
            var json = await _response.Content.ReadAsStringAsync();
            var actualGroup = JsonSerializer.Deserialize<ViewModels.GetGroup>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(_expectedGroup.Name, actualGroup.Name);
            Assert.Equal(_expectedGroup.Description, actualGroup.Description);
        }
    }
}