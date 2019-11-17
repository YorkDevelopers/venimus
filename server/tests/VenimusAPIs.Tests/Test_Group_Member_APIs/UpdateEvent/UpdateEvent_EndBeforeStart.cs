using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to update the details of an existing event", SoThat = "People are kept informed")]
    public class UpdateEvent_EndBeforeStart : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty;
        private Event _event;
        private Group _group;
        private ViewModels.UpdateEvent _amendedEvent;

        public UpdateEvent_EndBeforeStart(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "You cannot create an event which ends before it starts." },
                { Cultures.Test, "'€'You cannot create an event which ends before it starts." },
            }).BDDfy();
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

        private async Task WhenICallTheUpdateEventApiWithAnInvalidSlug()
        {
            _amendedEvent = Data.Create<ViewModels.UpdateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(-2);
            });

            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_group.Slug}/Events/{_event.Slug}", _amendedEvent);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("EndTimeUTC", ExpectedMessage);
        }

        private async Task ThenTheEventIsNotUpdatedInTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.Equal(_event.Slug, actualEvent.Slug);
            Assert.Equal(_event.Title, actualEvent.Title);
            Assert.Equal(_event.Description, actualEvent.Description);
            AssertDateTime(_event.StartTimeUTC, actualEvent.StartTimeUTC);
            AssertDateTime(_event.EndTimeUTC, actualEvent.EndTimeUTC);
            Assert.Equal(_event.Location, actualEvent.Location);
            Assert.Equal(_event.GroupId, actualEvent.GroupId);
            Assert.Equal(_event.GroupName, actualEvent.GroupName);
            Assert.Equal(_event.GroupSlug, actualEvent.GroupSlug);
        }
    }
}
