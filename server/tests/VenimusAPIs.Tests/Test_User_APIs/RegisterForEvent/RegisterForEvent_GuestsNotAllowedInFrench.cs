using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.RegisterForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class RegisterForEvent_GuestsNotAllowedInFrench : BaseTest
    {
        private Group _existingGroup;
        private Event _existingEvent;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_GuestsNotAllowedInFrench(Fixture fixture) : base(fixture)
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

        private async Task GivenAnEventExistsWhichDoesNotAllowGuests()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, e =>
            {
                e.GuestsAllowed = false;
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApiWithGuestsAndIHaveATestCluture()
        {
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();
            _signUpToEvent.EventSlug = _existingEvent.Slug;
            _signUpToEvent.NumberOfGuests = 2;

            Fixture.APIClient.SetCulture("zu-ZA");
            Response = await Fixture.APIClient.PostAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events", _signUpToEvent);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("NumberOfGuests", "'€'This event does not allow you to bring guests.  All attendees must be members of this group.");
        }

        private async Task ThenTheUserIsNotAMemberOfTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Empty(actualEvent.Members);
        }
    }
}
