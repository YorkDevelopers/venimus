using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to update the details of an existing event", SoThat = "People are kept informed")]
    public class UpdateEvent_UnknownGroup : BaseTest
    {
        public UpdateEvent_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAUser()
        {
            await IAmANormalUser();
        }

        private async Task WhenICallTheUpdateEventApiForAnUnknownGroup()
        {
            var amendedEvent = Data.Create<ViewModels.UpdateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
            });

            Fixture.APIClient.SetBearerToken(Token);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/MADEUP/Events/MADEUP", amendedEvent);
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
