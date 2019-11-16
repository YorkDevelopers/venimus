using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewEventAttendees
{
    [Story(AsA = "User", IWant = "to be able to view the other signed up attendees of an event", SoThat = "I can belong to the community")]
    public class ViewEventAttendees_UnapprovedMember : BaseTest
    {
        private Event _event;
        private Group _existingGroup;
        private User _otherUserInGroup1;
        private User _otherUserInGroup2;
        private User _otherUserInGroup3;
        private User _otherUserNotInGroup1;
        private Event.EventAttendees _userRegistration;
        private Event.EventAttendees _otherUser1Registration;
        private Event.EventAttendees _otherUser2Registration;

        public ViewEventAttendees_UnapprovedMember(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmAUser() => IAmANormalUser();

        private async Task GivenThereAreOtherUsers()
        {
            _otherUserInGroup1 = Data.Create<Models.User>();
            _otherUserInGroup2 = Data.Create<Models.User>();
            _otherUserInGroup3 = Data.Create<Models.User>();
            _otherUserNotInGroup1 = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertManyAsync(new[] { _otherUserInGroup1, _otherUserInGroup2, _otherUserInGroup3, _otherUserNotInGroup1 });
        }

        private async Task GivenIAndSomeOthersBelongToTheGroupButIAmNotApproved()
        {
            _existingGroup = Data.Create<Models.Group>(g =>
            {
                Data.AddGroupMember(g, User);
                Data.AddGroupMember(g, _otherUserInGroup1);
                Data.AddGroupMember(g, _otherUserInGroup2);
                Data.AddGroupAdministrator(g, _otherUserInGroup3);
            });

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenIAndSomeOthersAreGoingToAnEvent()
        {
            _event = Data.CreateEvent(_existingGroup, e =>
            {
                _userRegistration = Data.AddEventHost(e, User);
                _otherUser1Registration = Data.AddEventSpeaker(e, _otherUserInGroup1);
                _otherUser2Registration = Data.AddEventAttendee(e, _otherUserInGroup2);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_existingGroup.Slug}/Events/{_event.Slug}/Members");
        }

        private void ThenForbiddenIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }
    }
}
