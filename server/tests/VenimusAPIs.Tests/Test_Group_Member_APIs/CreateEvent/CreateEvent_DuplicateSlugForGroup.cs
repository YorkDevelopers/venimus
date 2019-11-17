using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to schedule a new event", SoThat = "People can meet up")]
    public class CreateEvent_DuplicateSlugForGroup : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty;
        private ViewModels.CreateEvent _event;
        private Group _group;
        private Event _existingEvent;

        public CreateEvent_DuplicateSlugForGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "An event with this slug already exists for this group." },
                { Cultures.Test, "'€'An event with this slug already exists for this group." },
            }).BDDfy();
        }

        private async Task GivenIAmANormalUser()
        {
            await IAmANormalUser();
        }

        private async Task GivenIAmAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_group, User);

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task GivenAnEventAlreadyExistsForTheGroup()
        {
            _existingEvent = Data.CreateEvent(_group);

            var collection = EventsCollection();

            await collection.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheCreateEventApiWithADuplicateSlug()
        {
            _event = Data.Create<ViewModels.CreateEvent>(e =>
            {
                e.Slug = _existingEvent.Slug;
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
            });

            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/events", _event);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("Slug", ExpectedMessage);
        }

        private async Task ThenTheEventIsNotAddedToTheDatabase()
        {
            var events = EventsCollection();
            var numberOfActualEvents = await events.Find(u => u.Slug == _event.Slug).CountDocumentsAsync();

            Assert.Equal(1, numberOfActualEvents);
        }
    }
}
