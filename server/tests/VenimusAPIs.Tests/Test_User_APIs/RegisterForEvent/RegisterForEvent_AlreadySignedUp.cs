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
    public class RegisterForEvent_AlreadySignedUp : BaseTest
    {
        private string _token;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;
        private Event _existingEvent;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_AlreadySignedUp(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);

            _user = Data.Create<Models.User>();
            var collection = UsersCollection();
            _user.Identities = new List<string> { _uniqueID };
            await collection.InsertOneAsync(_user);
        }

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, _user);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsAndIAmAlreadySignedUp()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, e =>
            {
                Data.AddEventAttendee(e, _user);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();
            _signUpToEvent.EventSlug = _existingEvent.Slug;

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PostAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events", _signUpToEvent);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequestDetail("You are already signed up to this event.");
        }
    }
}
