using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.GetGroupsLogo
{
    [Story(AsA = "User", IWant = "To be able to retrieve the logo for a group", SoThat = "I can join a community")]
    public class GetGroupLogo_Success : BaseTest
    {
        private Group _group1;

        public GetGroupLogo_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private Task GivenIAmNormalUser() => IAmANormalUser();

        private async Task GivenAGroupsExists()
        {
            _group1 = Data.Create<Models.Group>();

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_group1);
        }

        private async Task WhenICallTheGetGroupLogoApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_group1.Slug}/Logo");
            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheLogoIsReturned()
        {
            var actualLogo = await Response.Content.ReadAsByteArrayAsync();
            Assert.Equal(_group1.Logo, actualLogo);
        }
    }
}
