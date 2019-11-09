using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewEvent
{
    [Story(AsA = "ViewEvent", IWant = "To be able to view an existing event", SoThat = "I know the details")]
    public class ViewEvent_Success : BaseTest
    {
        private string _token;
        private Event _event;
        private Group _group;
        private User _user;

        public ViewEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmAUser()
        {
            var uniqueID = Guid.NewGuid().ToString();
            _token = await Fixture.GetTokenForNormalUser(uniqueID);

            _user = Data.Create<Models.User>();

            var collection = UsersCollection();
            _user.Identities = new List<string> { uniqueID };
            await collection.InsertOneAsync(_user);
        }

        private async Task GivenIAmAMemberOfTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupMember(_group, _user);

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
            Fixture.APIClient.SetBearerToken(_token);
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
            Assert.Equal(_event.MaximumNumberOfAttendees, actualEvent.MaximumNumberOfAttendees);
            Assert.Equal(_event.GuestsAllowed, actualEvent.GuestsAllowed);
            Assert.Equal(_event.Description, actualEvent.EventDescription);
            Assert.Equal(_event.Location, actualEvent.EventLocation);
            AssertDateTime(_event.StartTimeUTC, actualEvent.EventStartsUTC);
            AssertDateTime(_event.EndTimeUTC, actualEvent.EventFinishesUTC);
        }
    }
}
