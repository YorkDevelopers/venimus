using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Extensions;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AmendRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class AmendRegistrationForEvent_NegativeNumberOfGuests : BaseTest
    {
        private Group _existingGroup;
        private GroupEvent _existingEvent;
        private ViewModels.RegisterForEvent _amendedDetails;
        private GroupEventAttendees _currentRegistration;

        public AmendRegistrationForEvent_NegativeNumberOfGuests(Fixture fixture) : base(fixture)
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
                _currentRegistration = Data.AddEventAttendee(evt, User, numberOfGuests: 5);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApiWithANegativeNubmerOfGuests()
        {
            _amendedDetails = Data.Create<ViewModels.RegisterForEvent>();
            _amendedDetails.AddAnswer("NumberOfGuests", "-3");
            _amendedDetails.AddAnswer("DietaryRequirements", "ExampleDietaryRequirements");
            _amendedDetails.AddAnswer("MessageToOrganiser", "ExampleMessageToOrganiser");

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}", _amendedDetails);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("NumberOfGuests", "The NumberOfGuests cannot be less than zero.");
        }

        private async Task ThenTheUsersRegistrationIsNotUpdated()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Single(actualEvent.Members);

            var member = actualEvent.Members[0];
            Assert.Equal(User.Id.ToString(), member.UserId.ToString());
            Assert.Equal(_currentRegistration.DietaryRequirements, member.DietaryRequirements);
            Assert.Equal(_currentRegistration.NumberOfGuests, member.NumberOfGuests);
            Assert.True(member.SignedUp);
        }
    }
}
