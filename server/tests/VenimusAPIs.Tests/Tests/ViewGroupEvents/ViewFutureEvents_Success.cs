using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewGroupEvents
{
    [Story(AsA = "An user", IWant = "To be able to view events for a group", SoThat = "I learn about local groups and events")]
    public class ViewFutureEvents_Success : BaseTest
    {
        private Group _group;
        private Group _otherGroup;
        private GroupEvent[] _pastEvents;
        private GroupEvent[] _futureEvents;
        private GroupEvent[] _futureEventsOtherGroup;

        public ViewFutureEvents_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private async Task GivenOurGroupAndOtherGroupExists()
        {
            _group = Data.Create<Models.Group>();
            _otherGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();
            await groups.InsertManyAsync(new[] { _group, _otherGroup });
        }

        private async Task GivenThereAreSomeEvents()
        {
            _pastEvents = Enumerable.Range(1, 10)
                          .Select(day => Data.CreateEvent(_group, e =>
                          {
                              e.StartTimeUTC = DateTime.UtcNow.AddDays(-day);
                          }))
                          .ToArray();

            _futureEvents = Enumerable.Range(1, 20)
                                      .Select(day => Data.CreateEvent(_group, e =>
                                      {
                                          e.StartTimeUTC = DateTime.UtcNow.AddDays(day);
                                      }))
                                      .ToArray();

            _futureEventsOtherGroup = Enumerable.Range(1, 50)
                                      .Select(day => Data.CreateEvent(_otherGroup, e =>
                                      {
                                          e.StartTimeUTC = DateTime.UtcNow.AddDays(day);
                                      }))
                                      .ToArray();

            var events = EventsCollection();
            await events.InsertManyAsync(_pastEvents);
            await events.InsertManyAsync(_futureEvents);
            await events.InsertManyAsync(_futureEventsOtherGroup);
        }

        private async Task WhenICallTheAPI()
        {
            Fixture.APIClient.ClearBearerToken();
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_group.Slug}/Events");

            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheFutureEventsForTheGroupAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<ListEventsForGroup[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(20, events.Length);
            for (int i = 0; i < 20; i++)
            {
                AssertEvent(events, _futureEvents[i]);
            }
        }

        private void AssertEvent(ListEventsForGroup[] actualEvents, GroupEvent expectedEvent)
        {
            var actualEvent = actualEvents.Single(e => e.EventSlug == expectedEvent.Slug);

            Assert.Equal(expectedEvent.Title, actualEvent.EventTitle);
            Assert.Equal(expectedEvent.Description, actualEvent.EventDescription);
            AssertDateTime(expectedEvent.StartTimeUTC, actualEvent.EventStartsUTC);
            AssertDateTime(expectedEvent.EndTimeUTC, actualEvent.EventFinishesUTC);
        }
    }
}
