using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to schedule a new event", SoThat = "People can meet up")]
    public class CreateEvent_Success : BaseTest
    {
        private ViewModels.CreateEvent _event;
        private Group _group;

        public CreateEvent_Success(Fixture fixture) : base(fixture)
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

        private async Task WhenICallTheCreateEventApi()
        {
            _event = Data.Create<ViewModels.CreateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
            });

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/events", _event);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, Response.StatusCode);
        }

        private void ThenTheLocationOfTheNewEventIsReturned()
        {
            var location = Response.Headers.Location.ToString();
            Assert.Equal($"http://localhost/api/groups/{_group.Slug}/events/{_event.Slug}", location);
        }

        private async Task AndANewEventIsAddedToTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Slug == _event.Slug).SingleAsync();

            Assert.Equal(_event.Slug, actualEvent.Slug);
            Assert.Equal(_event.MaximumNumberOfAttendees, actualEvent.MaximumNumberOfAttendees);
            Assert.Equal(_event.Title, actualEvent.Title);
            Assert.Equal(_event.Description, actualEvent.Description);
            Assert.Equal(_event.GuestsAllowed, actualEvent.GuestsAllowed);
            AssertDateTime(_event.StartTimeUTC, actualEvent.StartTimeUTC);
            AssertDateTime(_event.EndTimeUTC, actualEvent.EndTimeUTC);
            Assert.Equal(_event.Location, actualEvent.Location);
            Assert.Equal(_group.Id, actualEvent.GroupId);
            Assert.Equal(_group.Slug, actualEvent.GroupSlug);
            Assert.Equal(_group.Name, actualEvent.GroupName);
        }
    }
}
