using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Extensions;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.RegisterForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class RegisterForEvent_Success : BaseTest
    {
        private Group _existingGroup;
        private GroupEvent _existingEvent;
        private ViewModels.RegisterForEvent _signUpToEvent;

        public RegisterForEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
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

        private async Task GivenAnEventExistsForThatGroup()
        {
            _existingEvent = Data.CreateEvent(_existingGroup);

            var events = EventsCollection();
            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            _signUpToEvent = Data.Create<ViewModels.RegisterForEvent>();
            _signUpToEvent.AddAnswer("NumberOfGuests", "5");
            _signUpToEvent.AddAnswer("DietaryRequirements", "ExampleDietaryRequirements");
            _signUpToEvent.AddAnswer("Q1", "Answer1");
            _signUpToEvent.AddAnswer("Q2", "Answer2");

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}", _signUpToEvent);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, Response.StatusCode);
        }

        private void ThenThePathToTheEventRegistrationIsReturned()
        {
            Assert.Equal($"http://localhost/api/user/groups/{_existingGroup.Slug}/events/{_existingEvent.Slug}", Response.Headers.Location.ToString());
        }

        private async Task ThenTheUserIsNowAMemberOfTheEvent()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Single(actualEvent.Members);

            var member = actualEvent.Members[0];
            Assert.Equal(User.Id.ToString(), member.UserId.ToString());
            Assert.Equal("ExampleDietaryRequirements", member.DietaryRequirements);
            Assert.Equal(5, member.NumberOfGuests);

            Assert.Equal(User.Bio, member.Bio);
            Assert.Equal(User.DisplayName, member.DisplayName);
            Assert.Equal(User.EmailAddress, member.EmailAddress);
            Assert.Equal(User.Fullname, member.Fullname);
            Assert.Equal(User.Pronoun, member.Pronoun);
            Assert.Equal(User.IsApproved, member.IsUserApproved);

            Assert.True(member.SignedUp);

            Assert.Equal(2, member.Answers.Count());
            AssertAnswer("Q1", "Caption1", "Answer1", member.Answers);
            AssertAnswer("Q2", "Caption2", "Answer2", member.Answers);
        }

        private void AssertAnswer(string expectedCode, string expectedCaption, string expectedAnswer, List<Answer> answers)
        {
            var actualAnswer = answers.Single(a => a.Code == expectedCode);
            Assert.Equal(expectedCaption, actualAnswer.Caption);
            Assert.Equal(expectedAnswer, actualAnswer.UsersAnswer);
        }
    }
}
