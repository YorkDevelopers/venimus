using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ListGroups
{
    [Story(AsA = "User", IWant = "To be able to retrieve the list of all groups", SoThat = "I can join a community")]
    public class ListGroups_Success : BaseTest
    {
        private Group _expectedGroup1;
        private Group _expectedGroup2;
        private Group _expectedGroup3;
        private Group _expectedGroup4;

        public ListGroups_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private Task GivenIAmNormalUser() => IAmANormalUser();

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
            Response = await Fixture.APIClient.GetAsync($"api/Groups");

            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheListOfGroupsIsReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var actualGroups = JsonSerializer.Deserialize<ViewModels.ListGroups[]>(json, options);

            AssertGroup(_expectedGroup1, actualGroups);
            AssertGroup(_expectedGroup2, actualGroups);
            AssertGroup(_expectedGroup3, actualGroups);
        }

        private void AssertGroup(Group expectedGroup, ViewModels.ListGroups[] actualGroups)
        {
            var actualGroup = actualGroups.Single(g => g.Slug == expectedGroup.Slug);

            Assert.Equal(expectedGroup.Name, actualGroup.Name);
            Assert.Equal(expectedGroup.Description, actualGroup.Description);
            Assert.Equal(expectedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(expectedGroup.IsActive, actualGroup.IsActive);
            Assert.Equal($"http://localhost/api/groups/{expectedGroup.Slug}/logo", actualGroup.Logo);
        }
    }
}
