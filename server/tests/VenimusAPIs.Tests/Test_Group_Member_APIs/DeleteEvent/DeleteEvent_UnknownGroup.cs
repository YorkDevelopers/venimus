using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to delete an existing event", SoThat = "People are kept informed")]
    public class DeleteEvent_UnknownGroup : BaseTest
    {
        private string _uniqueID;
        private string _token;
        private User _user;

        public DeleteEvent_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = await Fixture.GetTokenForNormalUser(_uniqueID);

            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task WhenICallTheDeleteEventApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/MADEUP/Events/MADEUP");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
