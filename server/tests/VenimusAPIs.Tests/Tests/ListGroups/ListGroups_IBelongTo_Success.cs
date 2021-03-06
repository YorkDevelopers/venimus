using MongoDB.Driver;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ListGroups
{
    [Story(AsA = "User", IWant = "To be able to see what groups I'm a member of", SoThat = "I can belong to the communities")]
    public class ListGroups_IBelongTo_Success : BaseTest
    {
        private Group _inGroup1;
        private Group _inGroup2;
        private Group _groupNotActive;
        private Group _notInGroup;
        private bool _userIsApproved = false;

        public ListGroups_IBelongTo_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            _userIsApproved = false;

            this.WithExamples(new ExampleTable("_userIsApproved")
            {
                { false },
                { true },
            }).BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser(_userIsApproved);

        private async Task GivenGroupsToWhichIBelongToExists()
        {
            _inGroup1 = Data.Create<Models.Group>(g =>
            {
                g.IsActive = true;
                Data.AddGroupMember(g, User);
            });

            _groupNotActive = Data.Create<Models.Group>(g =>
            {
                g.IsActive = false;
                Data.AddGroupMember(g, User);
            });

            _notInGroup = Data.Create<Models.Group>(g =>
            {
                g.IsActive = true;
            });

            _inGroup2 = Data.Create<Models.Group>(g =>
            {
                Data.AddGroupMember(g, User);
                g.IsActive = true;
            });

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_inGroup1);
            await groups.InsertOneAsync(_groupNotActive);
            await groups.InsertOneAsync(_notInGroup);
            await groups.InsertOneAsync(_inGroup2);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups?GroupsIBelongToOnly=true");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheListOfActiveGroupsTheUserBelongsToAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var groups = JsonSerializer.Deserialize<ViewModels.ListGroups[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(2, groups.Length);
            AssertGroup(groups, _inGroup1, _userIsApproved);
            AssertGroup(groups, _inGroup2, _userIsApproved);
        }

        private void AssertGroup(ViewModels.ListGroups[] actualGroups, Group expectedGroup, bool approvedGroupMember)
        {
            var actualGroup = actualGroups.Single(e => e.Slug == expectedGroup.Slug);

            Assert.Equal(expectedGroup.Slug, actualGroup.Slug);
            Assert.Equal(expectedGroup.Name, actualGroup.Name);
            Assert.Equal(expectedGroup.Description, actualGroup.Description);
            Assert.Equal(expectedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal($"http://localhost/api/groups/{expectedGroup.Slug}/logo", actualGroup.Logo.ToString());
        }
    }
}
