using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.RegisterForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class RegisterForEvent_AlreadyHappened : BaseTest
    {
        private Group _existingGroup;
        private Event _existingEvent;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_AlreadyHappened(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, User);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsWhichHasAlreadyTakenPlace()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                evt.EndTimeUTC = DateTime.UtcNow.AddDays(-10);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();
            _signUpToEvent.EventSlug = _existingEvent.Slug;
            _signUpToEvent.NumberOfGuests = 0;

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events", _signUpToEvent);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequestDetail("This event has already taken place");
        }

        private async Task ThenTheUserIsNotAMemberOfTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Empty(actualEvent.Members);
        }
    }
}
