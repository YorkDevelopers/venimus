using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AmendRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class AmendRegistrationForEvent_AlreadyHappened : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty;
        private Group _existingGroup;
        private Event _existingEvent;
        private ViewModels.AmendRegistrationForEvent _amendedDetails;
        private Event.EventAttendees _currentRegistration;

        public AmendRegistrationForEvent_AlreadyHappened(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "This event has already taken place" },
                { Cultures.Test, "'\u20AC'This event has already taken place" },
            }).BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, User);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsInThePastForThatGroupAndIWent()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                evt.EndTimeUTC = DateTime.UtcNow.AddDays(-10);
                _currentRegistration = Data.AddEventAttendee(evt, User, numberOfGuests: 5);
            });

            var events = EventsCollection();
            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            _amendedDetails = Data.Create<ViewModels.AmendRegistrationForEvent>();
            _amendedDetails.NumberOfGuests = 5;

            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}", _amendedDetails);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequestDetail(ExpectedMessage);
        }

        private async Task ThenTheUsersRegistrationIsNotUpdated()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Single(actualEvent.Members);

            var member = actualEvent.Members[0];
            Assert.Equal(User.Id.ToString(), member.UserId.ToString());
            Assert.Equal(_currentRegistration.DietaryRequirements, member.DietaryRequirements);
            Assert.Equal(_currentRegistration.MessageToOrganiser, member.MessageToOrganiser);
            Assert.Equal(_currentRegistration.NumberOfGuests, member.NumberOfGuests);
            Assert.True(member.SignedUp);
        }
    }
}
