using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AmendRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class AmendRegistrationForEvent_GuestsNotAllowed : BaseTest
    {
        private string _token;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;
        private Event _existingEvent;
        private ViewModels.AmendRegistrationForEvent _amendedDetails;
        private Event.EventAttendees _currentRegistration;

        public AmendRegistrationForEvent_GuestsNotAllowed(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);

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

        private async Task GivenAnEventExistsForThatGroupAndIAmGoingWhichDoesNotAllowGuests()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                _currentRegistration = Data.AddEventAttendee(evt, _user, numberOfGuests: 5);
                evt.GuestsAllowed = false;
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApiWithGuests()
        {
            _amendedDetails = Data.Create<ViewModels.AmendRegistrationForEvent>();
            _amendedDetails.NumberOfGuests = 1;

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}", _amendedDetails);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("NumberOfGuests", "This event does not allow you to bring guests.  All attendees must be members of this group.");
        }

        private async Task ThenTheUsersRegistrationIsNotUpdated()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Single(actualEvent.Members);

            var member = actualEvent.Members[0];
            Assert.Equal(_user.Id.ToString(), member.UserId.ToString());
            Assert.Equal(_currentRegistration.DietaryRequirements, member.DietaryRequirements);
            Assert.Equal(_currentRegistration.MessageToOrganiser, member.MessageToOrganiser);
            Assert.Equal(_currentRegistration.NumberOfGuests, member.NumberOfGuests);
            Assert.True(member.SignedUp);
        }
    }
}
