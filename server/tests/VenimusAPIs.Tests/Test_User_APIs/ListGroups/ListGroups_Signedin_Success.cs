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
    public class ListGroups_Signedin_Success : BaseTest
    {
        private Group _expectedActiveGroup1;
        private Group _expectedActiveGroup2;
        private Group _expectedActiveGroup3;
        private Group _expectedInactiveGroup;

        public ListGroups_Signedin_Success(Fixture fixture) : base(fixture)
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
            _expectedActiveGroup1 = Data.Create<Models.Group>();
            _expectedActiveGroup1.IsActive = true;

            _expectedActiveGroup2 = Data.Create<Models.Group>();
            _expectedActiveGroup2.IsActive = true;

            _expectedActiveGroup3 = Data.Create<Models.Group>();
            _expectedActiveGroup3.IsActive = true;

            _expectedInactiveGroup = Data.Create<Models.Group>();
            _expectedInactiveGroup.IsActive = false;

            var groups = GroupsCollection();

            await groups.InsertManyAsync(new[] { _expectedActiveGroup1, _expectedActiveGroup2, _expectedActiveGroup3, _expectedInactiveGroup });
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

            Assert.Equal(3, actualGroups.Length);
            AssertGroup(_expectedActiveGroup1, actualGroups);
            AssertGroup(_expectedActiveGroup2, actualGroups);
            AssertGroup(_expectedActiveGroup3, actualGroups);
        }

        private void AssertGroup(Group expectedGroup, ViewModels.ListGroups[] actualGroups)
        {
            var actualGroup = actualGroups.Single(g => g.Slug == expectedGroup.Slug);

            Assert.Equal(expectedGroup.Name, actualGroup.Name);
            Assert.Equal(expectedGroup.Description, actualGroup.Description);
            Assert.Equal(expectedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(expectedGroup.IsActive, actualGroup.IsActive);
            Assert.Equal(expectedGroup.StrapLine, actualGroup.StrapLine);
            Assert.Equal($"http://localhost/api/groups/{expectedGroup.Slug}/logo", actualGroup.Logo);
        }
    }
}
