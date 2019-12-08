using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewAllMyRegistrations
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class ViewAllMyRegistrations_Success : BaseTest
    {
        private Group _group1;
        private Group _group2;
        private GroupEvent _event1;
        private GroupEvent _event2;

        public ViewAllMyRegistrations_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenSomeGroupsExistsOfWhichIAmAMember()
        {
            _group1 = Data.Create<Models.Group>();
            Data.AddGroupMember(_group1, User);

            _group2 = Data.Create<Models.Group>();
            Data.AddGroupMember(_group2, User);

            var groups = GroupsCollection();

            await groups.InsertManyAsync(new[] { _group1, _group2 });
        }

        private async Task GivenAnEventExistsForThatGroupAndIAmGoing()
        {
            _event1 = Data.CreateEvent(_group1, evt =>
            {
                Data.AddEventAttendee(evt, User);
            });

            _event2 = Data.CreateEvent(_group2, evt =>
            {
                Data.AddEventAttendee(evt, User);
            });

            var events = EventsCollection();
            await events.InsertManyAsync(new[] { _event1, _event2, });
        }

        private async Task GivenIHaveAttendedAnEventInThePast()
        {
            var pastEvent = Data.CreateEvent(_group2, evt =>
            {
                evt.EndTimeUTC = DateTime.UtcNow.AddDays(-1);
                Data.AddEventAttendee(evt, User);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(pastEvent);
        }

        private async Task GivenThereAreEventsWhichIAmNotRegisteredFor()
        {
            var notRegisteredEvent = Data.CreateEvent(_group2, evt =>
            {
                Data.AddEventNonAttendee(evt, User);
            });

            var notRegisteredEvent2 = Data.CreateEvent(_group2, evt =>
            {
                evt.EndTimeUTC = DateTime.UtcNow.AddDays(1);
            });

            var events = EventsCollection();
            await events.InsertManyAsync(new[] { notRegisteredEvent, notRegisteredEvent2, });
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/user/Events");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheDetailsOfMyRegistrationAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualEvents = JsonSerializer.Deserialize<ViewAllMyEventRegistrations[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(2, actualEvents.Length);

            AssertEvent(actualEvents, _event1);
            AssertEvent(actualEvents, _event2);
        }

        private void AssertEvent(ViewAllMyEventRegistrations[] actualEvents, GroupEvent theEvent)
        {
            var actualEvent = actualEvents.Single(a => a.EventSlug == theEvent.Slug && a.GroupSlug == theEvent.GroupSlug);
            Assert.Equal(theEvent.GroupName, actualEvent.GroupName);
            Assert.Equal(theEvent.Description, actualEvent.EventDescription);
            AssertDateTime(theEvent.EndTimeUTC, actualEvent.EventFinishesUTC);
            AssertDateTime(theEvent.StartTimeUTC, actualEvent.EventStartsUTC);
            Assert.Equal(theEvent.Title, actualEvent.EventTitle);
        }
    }
}
