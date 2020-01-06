using MongoDB.Driver;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewEventAttendees
{
    [Story(AsA = "User", IWant = "to be able to view the other signed up attendees of an event", SoThat = "I can belong to the community")]
    public class ViewEventAttendees_Administrator_Success : BaseTest
    {
        private GroupEvent _event;
        private Group _existingGroup;
        private User _otherUserInGroup1;
        private User _otherUserInGroup2;
        private User _otherUserInGroup3;
        private User _otherUserNotInGroup1;
        private GroupEventAttendee _attendee1;
        private GroupEventAttendee _attendee2;

        public ViewEventAttendees_Administrator_Success(Fixture fixture) : base(fixture)
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

        private async Task GivenIAndSomeOthersBelongToTheGroup()
        {
            _existingGroup = Data.Create<Models.Group>(g =>
            {
                Data.AddGroupAdministrator(g, User);
                Data.AddGroupMember(g, _otherUserInGroup1);
                Data.AddGroupMember(g, _otherUserInGroup2);
                Data.AddGroupAdministrator(g, _otherUserInGroup3);
            });

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenSomeOthersAreGoingToAnEvent()
        {
            _event = Data.CreateEvent(_existingGroup, e =>
            {
                _attendee1 = Data.AddEventSpeaker(e, _otherUserInGroup1);
                _attendee2 = Data.AddEventAttendee(e, _otherUserInGroup2);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_existingGroup.Slug}/Events/{_event.Slug}/Members");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheMembersAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var actualAttendees = JsonSerializer.Deserialize<ViewModels.ListEventAttendees[]>(json, options);

            Assert.Equal(2, actualAttendees.Length);

            AssertMember(_otherUserInGroup1, _attendee1, actualAttendees, false, true);
            AssertMember(_otherUserInGroup2, _attendee2, actualAttendees, false, false);
        }

        private void AssertMember(User user, GroupEventAttendee attendee, ListEventAttendees[] actualAttendees, bool isHost, bool isSpeaker)
        {
            var actualAttendee = actualAttendees.Single(m => m.Slug == user.Id.ToString());

            Assert.Equal(user.DisplayName, actualAttendee.DisplayName);
            Assert.Equal(user.EmailAddress, actualAttendee.EmailAddress);
            Assert.Equal(user.Fullname, actualAttendee.Fullname);
            Assert.Equal(user.Bio, actualAttendee.Bio);
            Assert.Equal(user.Pronoun, actualAttendee.Pronoun);
            Assert.Equal(isHost, actualAttendee.IsHost);
            Assert.Equal(isSpeaker, actualAttendee.IsSpeaker);
            Assert.Equal($"http://localhost/api/users/{user.Id.ToString()}/profilepicture", actualAttendee.ProfilePicture.ToString());
            Assert.Equal(attendee.NumberOfGuests, actualAttendee.NumberOfGuests);

            Assert.Equal(attendee.DietaryRequirements, actualAttendee.DietaryRequirements);
            Assert.Equal(2, actualAttendee.Answers.Length);
            AssertAnswer("Q1", "Caption1", "Text", "Answer1", actualAttendee.Answers);
            AssertAnswer("Q2", "Caption2", "Text", "Answer2", actualAttendee.Answers);
        }

        private void AssertAnswer(string expectedCode, string expectedCaption, string expectedQuestionType, string expectedUsersAnswer, ViewModels.Answer[] allAnswers)
        {
            var actualAnswer = allAnswers.Single(a => a.Code == expectedCode);
            Assert.Equal(expectedCaption, actualAnswer.Caption);
            Assert.Equal(expectedQuestionType, actualAnswer.QuestionType);
            Assert.Equal(expectedUsersAnswer, actualAnswer.UsersAnswer);
        }
    }
}
