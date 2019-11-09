using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewEventAttendees
{
    [Story(AsA = "User", IWant = "to be able to view the other signed up attendees of an event", SoThat = "I can belong to the community")]
    public class ViewEventAttendees_UnknownGroup : BaseTest
    {
        public ViewEventAttendees_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmUser()
        {
            await IAmANormalUser();
        }

        private async Task WhenICallTheApiForAnUnknownGroup()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/MADEUP/Events/MADEUP/Members");
        }

        private void ThenNotFoundIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
