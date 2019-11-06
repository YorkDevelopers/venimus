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
    public class UpdateEvent_UnknownEvent : BaseTest
    {
        private string _uniqueID;
        private string _token;
        private Group _group;
        private ViewModels.UpdateEvent _amendedEvent;
        private User _user;

        public UpdateEvent_UnknownEvent(Fixture fixture) : base(fixture)
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

        private async Task GivenIAmAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_group, _user);

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task WhenICallTheUpdateEventApiForAnUnknownEvent()
        {
            _amendedEvent = Data.Create<ViewModels.UpdateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
            });

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_group.Slug}/Events/MADEUP", _amendedEvent);
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
