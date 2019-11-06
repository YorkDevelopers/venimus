using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewMyRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class ViewMyRegistrationForEvent_Success : BaseTest
    {
        private string _token;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;
        private Event _existingEvent;

        public ViewMyRegistrationForEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIAmUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);
        }

        private async Task GivenAlreadyExistInTheDatabase()
        {
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, _user);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsForThatGroupAndIAmGoing()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                evt.EndTimeUTC = DateTime.UtcNow.AddDays(1);
                evt.Members = new List<Event.EventAttendees>
                {
                    new Event.EventAttendees
                    {
                        SignedUp = true,
                        UserId = _user.Id,
                        DietaryRequirements = "milk free",
                        MessageToOrganiser = "My first time",
                        NumberOfGuests = 10,
                    },
                };
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.GetAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheDetailsOfTheRegistrationAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualRegistration = JsonSerializer.Deserialize<ViewMyEventRegistration>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("My first time", actualRegistration.MessageToOrganiser);
            Assert.Equal(10, actualRegistration.NumberOfGuests);
            Assert.Equal("milk free", actualRegistration.DietaryRequirements);
            Assert.True(actualRegistration.Attending);
        }
    }
}
