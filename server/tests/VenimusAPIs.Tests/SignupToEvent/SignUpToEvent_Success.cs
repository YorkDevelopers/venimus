using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class SignUpToEvent_Success : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private Group _existingGroup;
        private string _uniqueID;
        private User _user;
        private Event _existingEvent;
        private SignUpToEvent _signUpToEvent;

        public SignUpToEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIAmUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);
        }

        private async Task GivenAlreadyExistInTheDatabase()
        {
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            _existingGroup.Members = new List<MongoDB.Bson.ObjectId> { _user.Id };

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsForThatGroup()
        {
            _existingEvent = Data.Create<Models.Event>(evt =>
            {
                evt.GroupId = _existingGroup.Id;
                evt.GroupSlug = _existingGroup.Slug;
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            _signUpToEvent = Data.Create<ViewModels.SignUpToEvent>();
            _signUpToEvent.EventSlug = _existingEvent.Slug;

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PostAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events", _signUpToEvent);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, _response.StatusCode);
        }

        private void ThenThePathToTheEventRegistrationIsReturned()
        {
            Assert.Equal($"http://localhost/api/user/groups/{_existingGroup.Slug}/events/{_existingEvent.Slug}", _response.Headers.Location.ToString());
        }

        private async Task ThenTheUserIsNowAMemberOfTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Single(actualEvent.Members);

            var member = actualEvent.Members[0];
            Assert.Equal(_user.Id.ToString(), member.UserId.ToString());
            Assert.Equal(_signUpToEvent.DietaryRequirements, member.DietaryRequirements);
            Assert.Equal(_signUpToEvent.MessageToOrganiser, member.MessageToOrganiser);
            Assert.Equal(_signUpToEvent.NumberOfGuests, member.NumberOfGuests);
            Assert.True(member.SignedUp);
        }
    }
}
