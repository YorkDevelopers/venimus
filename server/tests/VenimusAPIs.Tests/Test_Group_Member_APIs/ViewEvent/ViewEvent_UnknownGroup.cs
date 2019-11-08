using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewEvent
{
    [Story(AsA = "ViewEvent", IWant = "To be able to view an existing event", SoThat = "I know the details")]
    public class ViewEvent_UnknownGroup : BaseTest
    {
        private string _token;

        public ViewEvent_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAUser()
        {
            var uniqueID = Guid.NewGuid().ToString();
            _token = await Fixture.GetTokenForNormalUser(uniqueID);

            var user = Data.Create<Models.User>();

            var collection = UsersCollection();
            await collection.InsertOneAsync(user);
        }

        private async Task WhenICallTheGetEventApiForAGroupWhichDoesNotExist()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/Groups/MADEUP/Events/MADEUP");
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
