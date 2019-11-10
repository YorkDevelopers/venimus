using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewEvent
{
    [Story(AsA = "ViewEvent", IWant = "To be able to view an existing event", SoThat = "I know the details")]
    public class ViewEvent_UnknownGroup : BaseTest
    {
        public ViewEvent_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmAUser() => IAmANormalUser();

        private async Task WhenICallTheGetEventApiForAGroupWhichDoesNotExist()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/MADEUP/Events/MADEUP");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
