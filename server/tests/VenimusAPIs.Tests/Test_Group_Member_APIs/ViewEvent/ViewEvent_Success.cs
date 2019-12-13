using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewEvent
{
    [Story(AsA = "ViewEvent", IWant = "To be able to view an existing event", SoThat = "I know the details")]
    public class ViewEvent_Success : BaseTest
    {
        private GroupEvent _event;
        private Group _group;

        public ViewEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmANormalUser() => IAmANormalUser();

        private async Task GivenIAmAMemberOfTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupMember(_group, User);

            var collection = GroupsCollection();
            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExistsForTheGroup()
        {
            _event = Data.CreateEvent(_group);

            var events = EventsCollection();
            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheGetEventApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheFullDetailsOfTheEventAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualEvent = JsonSerializer.Deserialize<GetEvent>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(_group.Name, actualEvent.GroupName);
            Assert.Equal(_event.Slug, actualEvent.EventSlug);
            Assert.Equal(_event.Title, actualEvent.EventTitle);
            Assert.Equal(_event.MaximumNumberOfAttendees, actualEvent.MaximumNumberOfAttendees);
            Assert.Equal(_event.GuestsAllowed, actualEvent.GuestsAllowed);
            Assert.Equal(_event.Description, actualEvent.EventDescription);
            Assert.Equal(_event.Location, actualEvent.EventLocation);
            AssertDateTime(_event.StartTimeUTC, actualEvent.EventStartsUTC);
            AssertDateTime(_event.EndTimeUTC, actualEvent.EventFinishesUTC);

            Assert.Equal(3, actualEvent.Questions.Length);

            var q0 = actualEvent.Questions.First(q => q.Code == "NumberOfGuests");
            Assert.Equal("Number of Guests", q0.Caption);
            Assert.Equal("NumberOfGuests", q0.QuestionType);

            var q1 = actualEvent.Questions.First(q => q.Code == "Q1");
            Assert.Equal("Dietary Requirements", q1.Caption);
            Assert.Equal("Text", q1.QuestionType);

            var q2 = actualEvent.Questions.First(q => q.Code == "Q2");
            Assert.Equal("Message to Organiser", q2.Caption);
            Assert.Equal("Text", q2.QuestionType);
        }
    }
}
