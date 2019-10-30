using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "User", IWant = "To be able to retrieve the list of all groups", SoThat = "I can join a community")]
    public class ListGroups : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private Group _expectedGroup1;
        private Group _expectedGroup2;
        private Group _expectedGroup3;
        private Group _expectedGroup4;

        public ListGroups(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAUser()
        {
            _token = await Fixture.GetToken();
        }

        private async Task GivenThatSeveralGroupsExists()
        {
            _expectedGroup1 = Data.Create<Models.Group>();
            _expectedGroup2 = Data.Create<Models.Group>();
            _expectedGroup3 = Data.Create<Models.Group>();
            _expectedGroup4 = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertManyAsync(new[] { _expectedGroup1, _expectedGroup2, _expectedGroup3, _expectedGroup4 });
        }

        private async Task WhenICallTheGetGroupApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.GetAsync($"api/Groups");

            _response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheListOfGroupsIsReturned()
        {
            var json = await _response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var actualGroups = JsonSerializer.Deserialize<ViewModels.ListGroups[]>(json, options);

            AssertGroup(_expectedGroup1, actualGroups);
            AssertGroup(_expectedGroup2, actualGroups);
            AssertGroup(_expectedGroup3, actualGroups);
        }

        private void AssertGroup(Group expectedGroup, ViewModels.ListGroups[] actualGroups)
        {
            var actualGroup = actualGroups.Single(g => g.Name == expectedGroup.Name);

            Assert.Equal(expectedGroup.Name, actualGroup.Name);
            Assert.Equal(expectedGroup.Description, actualGroup.Description);
        }
    }
}
