﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;
using System.Text.Json;
using VenimusAPIs.ViewModels;
using System.Linq;
using VenimusAPIs.Models;

namespace VenimusAPIs.Tests.PublicFutureEvents
{
    [Story(AsA = "An unauthenticated user", IWant = "To be able to view current events", SoThat = "I learn about local groups and events")]
    public class ListEvents : BaseTest
    {
        private HttpResponseMessage _response;
        private Group _group1;
        private Group _group2;
        private Group _group3;
        private Event _futureEvent1;
        private Event _futureEvent2;
        private Event _futureEvent3;

        public ListEvents(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenThereAreSomeEvents()
        {
            var pastEvent = Data.Create<Models.Event>();
            pastEvent.StartTime = DateTime.UtcNow.AddDays(-1);

            _group1 = Data.Create<Models.Group>();
            _group2 = Data.Create<Models.Group>();
            _group3 = Data.Create<Models.Group>();

            var groups = GroupsCollection();
            await groups.InsertManyAsync(new[] { _group1, _group2, _group3 });

            _futureEvent1 = Data.Create<Models.Event>();
            _futureEvent1.StartTime = DateTime.UtcNow.AddDays(1);
            _futureEvent1.GroupId = _group1.Id;

            _futureEvent2 = Data.Create<Models.Event>();
            _futureEvent2.StartTime = DateTime.UtcNow.AddDays(6);
            _futureEvent2.GroupId = _group2.Id;

            _futureEvent3 = Data.Create<Models.Event>();
            _futureEvent3.StartTime = DateTime.UtcNow.AddYears(2);
            _futureEvent3.GroupId = _group3.Id;

            var events = EventsCollection();
            await events.InsertManyAsync(new[] { pastEvent, _futureEvent1, _futureEvent2, _futureEvent3, });
        }

        private async Task WhenICallTheAPI()
        {
            Fixture.APIClient.ClearBearerToken();
            _response = await Fixture.APIClient.GetAsync("public/FutureEvents");

            _response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheFutureEventsAreReturned()
        {
            var json = await _response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<ListFutureEvents[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert.Equal(3, events.Length);
            AssertEvent(events, _futureEvent1, _group1);
            AssertEvent(events, _futureEvent2, _group2);
            AssertEvent(events, _futureEvent3, _group3);
        }

        private void AssertEvent(ListFutureEvents[] actualEvents, Event expectedEvent, Group expectedGroup)
        {
            var actualEvent = actualEvents.Single(e => e.EventId == expectedEvent.Id.ToString());

            Assert.Equal(expectedGroup.Name, actualEvent.GroupName);
            Assert.Equal(expectedEvent.Title, actualEvent.EventTitle);
            Assert.Equal(expectedEvent.Description, actualEvent.EventDescription);
            Assert.Equal(TrimMilliseconds(expectedEvent.StartTime), TrimMilliseconds(actualEvent.EventStartsUTC));
            Assert.Equal(TrimMilliseconds(expectedEvent.EndTime), TrimMilliseconds(actualEvent.EventFinishesUTC));
        }

        private DateTime TrimMilliseconds(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
        }
    }
}