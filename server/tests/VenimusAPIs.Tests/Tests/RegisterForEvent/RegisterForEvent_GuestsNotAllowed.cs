using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Extensions;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.RegisterForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class RegisterForEvent_GuestsNotAllowed : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty;
        private Group _existingGroup;
        private GroupEvent _existingEvent;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_GuestsNotAllowed(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "This event does not allow you to bring guests.  All attendees must be members of this group." },
                { Cultures.Test, "'\u20AC'This event does not allow you to bring guests.  All attendees must be members of this group." },
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

        private async Task GivenAnEventExistsWhichDoesNotAllowGuests()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, e =>
            {
                e.GuestsAllowed = false;
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApiWithGuests()
        {
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();
            _signUpToEvent.AddAnswer("NumberOfGuests", "2");
            _signUpToEvent.AddAnswer("DietaryRequirements", "ExampleDietaryRequirements");
            _signUpToEvent.AddAnswer("MessageToOrganiser", "ExampleMessageToOrganiser");

            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}", _signUpToEvent);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("NumberOfGuests", ExpectedMessage);
        }

        private async Task ThenTheUserIsNotAMemberOfTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Empty(actualEvent.Members);
        }
    }
}
