using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.RegisterForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class RegisterForEvent_Full : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty; 
        private Group _existingGroup;
        private GroupEvent _existingEvent;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_Full(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "Sorry this is event is full." },
                { Cultures.Test, "'\u20AC'Sorry this is event is full." },
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

        private async Task GivenAnEventExistsForThatGroupWhichIsFull()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, e =>
            {
                e.MaximumNumberOfAttendees = 10;

                // Ten people are already attending
                Data.AddEventAttendee(e, Data.Create<Models.User>(), numberOfGuests: 2);
                Data.AddEventAttendee(e, Data.Create<Models.User>(), numberOfGuests: 3);
                Data.AddEventAttendee(e, Data.Create<Models.User>());
                Data.AddEventAttendee(e, Data.Create<Models.User>());
                Data.AddEventAttendee(e, Data.Create<Models.User>());
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();
            _signUpToEvent.EventSlug = _existingEvent.Slug;

            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.PostAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events", _signUpToEvent);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequestDetail(ExpectedMessage);
        }

        private async Task ThenTheUserIsNotAMemberOfTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Equal(5, actualEvent.Members.Count);
        }
    }
}
