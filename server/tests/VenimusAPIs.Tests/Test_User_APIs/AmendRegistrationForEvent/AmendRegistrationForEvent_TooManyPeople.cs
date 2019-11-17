using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AmendRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class AmendRegistrationForEvent_TooManyPeople : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty; 
        private Group _existingGroup;
        private Event _existingEvent;
        private ViewModels.AmendRegistrationForEvent _amendedDetails;
        private Event.EventAttendees _currentRegistration;

        public AmendRegistrationForEvent_TooManyPeople(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "Sorry this will exceed the maximum number of people allowed to attend this event." },
                { Cultures.Test, "'€'Sorry this will exceed the maximum number of people allowed to attend this event." },
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

        private async Task GivenAnEventExistsForThatGroupAndIAmGoing()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                _currentRegistration = Data.AddEventAttendee(evt, User, numberOfGuests: 5);
                evt.MaximumNumberOfAttendees = 10;
                evt.GuestsAllowed = true;
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApiWithTooManyGuests()
        {
            _amendedDetails = Data.Create<ViewModels.AmendRegistrationForEvent>();
            _amendedDetails.NumberOfGuests = 10;

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
