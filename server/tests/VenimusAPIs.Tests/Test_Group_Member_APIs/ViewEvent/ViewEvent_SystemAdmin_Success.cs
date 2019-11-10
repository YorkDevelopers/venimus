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
    public class ViewEvent_SystemAdmin_Success : BaseTest
    {
        private Event _event;
        private Group _group;

        public ViewEvent_SystemAdmin_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmASystemAdministratorUser() => IAmASystemAdministrator();

        private async Task GivenIAmNotAMemberOfTheGroup()
        {
            _group = Data.Create<Models.Group>();

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
            Assert.Equal(_event.Description, actualEvent.EventDescription);
            Assert.Equal(_event.Location, actualEvent.EventLocation);
            AssertDateTime(_event.StartTimeUTC, actualEvent.EventStartsUTC);
            AssertDateTime(_event.EndTimeUTC, actualEvent.EventFinishesUTC);
        }
    }
}
