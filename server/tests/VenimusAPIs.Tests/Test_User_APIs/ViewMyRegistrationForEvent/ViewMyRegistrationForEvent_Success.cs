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
        private Group _existingGroup;
        private Event _existingEvent;
        private Event.EventAttendees _registrationDetails;

        public ViewMyRegistrationForEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, User);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsForThatGroupAndIAmGoing()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                _registrationDetails = Data.AddEventAttendee(evt, User, numberOfGuests: 10);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
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

            Assert.Equal(_registrationDetails.MessageToOrganiser, actualRegistration.MessageToOrganiser);
            Assert.Equal(10, actualRegistration.NumberOfGuests);
            Assert.Equal(_registrationDetails.DietaryRequirements, actualRegistration.DietaryRequirements);
            Assert.True(actualRegistration.Attending);
        }
    }
}
