using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.GetGroupLogo
{
    [Story(AsA = "User", IWant = "To be able to retrieve the logo for a group", SoThat = "I can join a community")]
    public class GetGroupLogo_UnknownGroup : BaseTest
    {
        public GetGroupLogo_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private async Task WhenICallTheGetGroupLogoApiForAnUnknownGroup()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/MADEUP/Logo");
        }

        private void ThenNotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
