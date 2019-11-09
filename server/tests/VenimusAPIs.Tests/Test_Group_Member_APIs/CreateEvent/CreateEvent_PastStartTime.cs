using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to schedule a new event", SoThat = "People can meet up")]
    public class CreateEvent_PastStartTime : BaseTest
    {
        private ViewModels.CreateEvent _event;
        private Group _group;

        public CreateEvent_PastStartTime(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
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

        private async Task WhenICallTheCreateEventApiForAnEventInThePast()
        {
            _event = Data.Create<ViewModels.CreateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(-1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
            });

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/events", _event);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("StartTimeUTC", "You cannot create an event in the past.");
        }

        private async Task ThenTheEventIsNotAddedToTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Slug == _event.Slug).SingleOrDefaultAsync();

            Assert.Null(actualEvent);
        }
    }
}
