using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewSingleGroupMembership
{
    [Trait("API", "GET api/User/Groups/{_theGroup.Slug}")]
    [Story(AsA = "User", IWant = "To be able to see the details of a single group I'm a member of", SoThat = "I can belong to the community")]
    public class ViewSingleGroupMembership_Success : BaseTest
    {
        private Group _theGroup;

        public ViewSingleGroupMembership_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenIBelongToAGroup()
        {
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");

            _theGroup = Data.Create<Models.Group>(g =>
            {
                Data.AddGroupMember(g, User);
                g.IsActive = true;
                g.Logo = logo;
            });

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_theGroup);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/User/Groups/{_theGroup.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheListOfActiveGroupsTheUserBelongsToAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualMembership = JsonSerializer.Deserialize<ViewMyGroupMembership>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(_theGroup.Slug, actualMembership.GroupSlug);
            Assert.Equal(_theGroup.Name, actualMembership.GroupName);
            Assert.Equal(_theGroup.Description, actualMembership.GroupDescription);
            Assert.Equal(_theGroup.SlackChannelName, actualMembership.GroupSlackChannelName);
            Assert.Equal(_theGroup.Logo, Convert.FromBase64String(actualMembership.GroupLogoInBase64));
        }
    }
}
