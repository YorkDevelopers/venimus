using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.PublicFutureEvents
{
    [Story(AsA = "An unauthenticated user", IWant = "To be able to view current events", SoThat = "I learn about local groups and events")]
    public class ListEvents : BaseTest
    {
        private Group _group1;
        private Group _group2;
        private Group _group3;
        private Event _futureEvent1;
        private Event _futureEvent2;
        private Event[] _futureEvents;

        public ListEvents(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private async Task GivenThereAreSomeEvents()
        {
            var pastEvent = Data.Create<Models.Event>();
            pastEvent.StartTimeUTC = DateTime.UtcNow.AddDays(-1);

            _group1 = Data.Create<Models.Group>();
            _group2 = Data.Create<Models.Group>();
            _group3 = Data.Create<Models.Group>();

            var groups = GroupsCollection();
            await groups.InsertManyAsync(new[] { _group1, _group2, _group3 });

            _futureEvent1 = Data.CreateEvent(_group1);
            _futureEvent1.StartTimeUTC = DateTime.UtcNow.AddDays(1);

            _futureEvent2 = Data.CreateEvent(_group2);
            _futureEvent2.StartTimeUTC = DateTime.UtcNow.AddDays(6);

            _futureEvents = Enumerable.Range(1, 20)
                                      .Select(day => Data.CreateEvent(_group3, e =>
                                      {
                                          e.StartTimeUTC = DateTime.UtcNow.AddDays(day);
                                      }))
                                      .ToArray();

            var events = EventsCollection();
            await events.InsertManyAsync(new[] { pastEvent, _futureEvent1, _futureEvent2 });
            await events.InsertManyAsync(_futureEvents);
        }

        private async Task WhenICallTheAPI()
        {
            Fixture.APIClient.ClearBearerToken();
            Response = await Fixture.APIClient.GetAsync("public/FutureEvents");

            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheNext10FutureEventsForEachGroupAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<ListFutureEvents[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(12, events.Length);

            AssertEvent(events, _futureEvent1, _group1);
            AssertEvent(events, _futureEvent2, _group2);

            for (int i = 0; i < 10; i++)
            {
                AssertEvent(events, _futureEvents[i], _group3);
            }
        }

        private void AssertEvent(ListFutureEvents[] actualEvents, Event expectedEvent, Group expectedGroup)
        {
            var actualEvent = actualEvents.Single(e => e.EventSlug == expectedEvent.Id.ToString());

            Assert.Equal(expectedGroup.Name, actualEvent.GroupName);
            Assert.Equal(expectedEvent.Title, actualEvent.EventTitle);
            Assert.Equal(expectedEvent.Description, actualEvent.EventDescription);
            AssertDateTime(expectedEvent.StartTimeUTC, actualEvent.EventStartsUTC);
            AssertDateTime(expectedEvent.EndTimeUTC, actualEvent.EventFinishesUTC);
        }
    }
}
