using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AmendRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class AmendRegistrationForEvent_UnknownGroup : BaseTest
    {
        private ViewModels.RegisterForEvent _amendedDetails;

        public AmendRegistrationForEvent_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task WhenICallTheApiForAnUnknownGroup()
        {
            _amendedDetails = Data.Create<ViewModels.RegisterForEvent>();

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user/groups/MADEUP/Events/MADEUP", _amendedDetails);
        }

        private void ThenNotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
