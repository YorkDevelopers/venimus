using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to delete an existing event", SoThat = "People are kept informed")]
    public class DeleteEvent_UnknownGroup : BaseTest
    {
        public DeleteEvent_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmANormalUser()
        {
            await IAmANormalUser();
        }

        private async Task WhenICallTheDeleteEventApi()
        {
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/MADEUP/Events/MADEUP");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
