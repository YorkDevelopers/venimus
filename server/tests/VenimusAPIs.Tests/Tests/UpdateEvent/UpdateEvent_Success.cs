using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to update the details of an existing event", SoThat = "People are kept informed")]
    public class UpdateEvent_Success : BaseTest
    {
        private GroupEvent _event;
        private Group _group;
        private ViewModels.UpdateEvent _amendedEvent;

        public UpdateEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmANormalUser() => IAmANormalUser();

        private async Task GivenIAmAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_group, User);

            var collection = GroupsCollection();
            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExistsForTheGroup()
        {
            _event = Data.CreateEvent(_group);

            var events = EventsCollection();

            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheUpdateEventApi()
        {
            _amendedEvent = Data.Create<ViewModels.UpdateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
            });

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}", _amendedEvent);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenTheEventIsUpdatedInTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.Equal(_amendedEvent.Slug, actualEvent.Slug);
            Assert.Equal(_amendedEvent.MaximumNumberOfAttendees, actualEvent.MaximumNumberOfAttendees);
            Assert.Equal(_amendedEvent.GuestsAllowed, actualEvent.GuestsAllowed);
            Assert.Equal(_amendedEvent.Title, actualEvent.Title);
            Assert.Equal(_amendedEvent.Description, actualEvent.Description);
            AssertDateTime(_amendedEvent.StartTimeUTC, actualEvent.StartTimeUTC);
            AssertDateTime(_amendedEvent.EndTimeUTC, actualEvent.EndTimeUTC);
            Assert.Equal(_amendedEvent.Location, actualEvent.Location);
            Assert.Equal(_group.Id, actualEvent.GroupId);
            Assert.Equal(_group.Name, actualEvent.GroupName);
            Assert.Equal(_group.Slug, actualEvent.GroupSlug);
        }
    }
}
