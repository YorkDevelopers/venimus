using System;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.GetGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to retrieve existing groups", SoThat = "I can maintain them")]
    public class GetGroup_NotSignedIn_Success : BaseTest
    {
        private Models.Group _expectedGroup;

        public GetGroup_NotSignedIn_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenAGroupAlreadyExists()
        {
            _expectedGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_expectedGroup);
        }

        private async Task WhenICallTheGetGroupApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_expectedGroup.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheDetailsOfTheGroupAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualGroup = JsonSerializer.Deserialize<ViewModels.GetGroup>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(_expectedGroup.Slug, actualGroup.Slug);
            Assert.Equal(_expectedGroup.Name, actualGroup.Name);
            Assert.Equal(_expectedGroup.Description, actualGroup.Description);
            Assert.Equal(_expectedGroup.StrapLine, actualGroup.StrapLine);
            Assert.Equal(_expectedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.False(actualGroup.CanViewMembers);
            Assert.Equal(_expectedGroup.IsActive, actualGroup.IsActive);
            Assert.Equal($"http://localhost/api/groups/{_expectedGroup.Slug}/logo", actualGroup.Logo.ToString());
        }
    }
}
