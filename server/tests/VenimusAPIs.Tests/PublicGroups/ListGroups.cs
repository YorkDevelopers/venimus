using System;
using System.Net.Http;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;
using System.Text.Json;
using VenimusAPIs.ViewModels;
using System.Linq;
using VenimusAPIs.Models;

namespace VenimusAPIs.Tests.PublicGroups
{
    [Story(AsA = "An unauthenticated user", IWant = "To be able to view the list of active groups", SoThat = "I learn about local groups and events")]
    public class ListGroups : BaseTest
    {
        private HttpResponseMessage _response;
        private Group _group1;
        private Group _group2;
        private Group _group3;

        public ListGroups(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private async Task GivenThereAreSomeGroups()
        {
            var pastEvent = Data.Create<Models.Event>();
            pastEvent.StartTime = DateTime.UtcNow.AddDays(-1);

            _group1 = Data.Create<Models.Group>();
            _group1.IsActive = false;

            _group2 = Data.Create<Models.Group>();
            _group2.IsActive = true;

            _group3 = Data.Create<Models.Group>();
            _group3.IsActive = true;

            var groups = GroupsCollection();
            await groups.InsertManyAsync(new[] { _group1, _group2, _group3 });
        }

        private async Task WhenICallTheAPI()
        {
            Fixture.APIClient.ClearBearerToken();
            _response = await Fixture.APIClient.GetAsync("public/Groups");

            _response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheActiveGroupsAreReturned()
        {
            var json = await _response.Content.ReadAsStringAsync();
            var groups = JsonSerializer.Deserialize<ListActiveGroups[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(2, groups.Length);
            AssertGroup(groups, _group2);
            AssertGroup(groups, _group3);
        }

        private void AssertGroup(ListActiveGroups[] actualGroups, Group expectedGroup)
        {
            var actualGroup = actualGroups.Single(e => e.Slug == expectedGroup.Slug);

            Assert.Equal(expectedGroup.Slug, actualGroup.Slug);
            Assert.Equal(expectedGroup.Name, actualGroup.Name);
            Assert.Equal(expectedGroup.Description, actualGroup.Description);
            Assert.Equal(expectedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(expectedGroup.Logo, Convert.FromBase64String(actualGroup.LogoInBase64));
        }
    }
}
