using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.RegisterForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class RegisterForEvent_NegativeNumberOfGuests : BaseTest
    {
        private Group _existingGroup;
        private Event _existingEvent;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_NegativeNumberOfGuests(Fixture fixture) : base(fixture)
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

        private async Task GivenAnEventExists()
        {
            _existingEvent = Data.CreateEvent(_existingGroup);

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApiWithANegativeNumberOfGuests()
        {
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();
            _signUpToEvent.EventSlug = _existingEvent.Slug;
            _signUpToEvent.NumberOfGuests = -2;

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events", _signUpToEvent);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("NumberOfGuests", "The field NumberOfGuests must be between 0 and 2147483647.");
        }

        private async Task ThenTheUserIsNotAMemberOfTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Empty(actualEvent.Members);
        }
    }
}
