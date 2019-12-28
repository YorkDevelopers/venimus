using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using VenimusAPIs.Tests.Extensions;
using Xunit;

namespace VenimusAPIs.Tests.ViewMyRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class ViewMyRegistrationForEvent_Success : BaseTest
    {
        private Group _existingGroup;
        private GroupEvent _existingEvent;
        private GroupEventAttendee _registrationDetails;

        public ViewMyRegistrationForEvent_Success(Fixture fixture) : base(fixture)
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

        private async Task GivenAnEventExistsForThatGroupAndIAmGoing()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                _registrationDetails = Data.AddEventAttendee(evt, User, numberOfGuests: 10);
            });

            _registrationDetails.AddAnswer("Q1", "Answer1");
            _registrationDetails.AddAnswer("Q2", "Answer2");
            _registrationDetails.AddAnswer("Q3", "Answer3");

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheDetailsOfTheRegistrationAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualRegistration = JsonSerializer.Deserialize<ViewMyEventRegistration>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.True(actualRegistration.Attending);
            Assert.Equal(_existingGroup.Name, actualRegistration.GroupName);
            Assert.Equal(_existingEvent.Title, actualRegistration.EventTitle);

            Assert.Equal(4, actualRegistration.Answers.Length);
            AssertAnswer("NumberOfGuests", "Number of Guests", "NumberOfGuests", "10", actualRegistration.Answers);
            AssertAnswer("DietaryRequirements", "Any dietary requirements?", "DietaryRequirements", "No milk", actualRegistration.Answers);
            AssertAnswer("Q1", "Caption1", "Text", "Answer1", actualRegistration.Answers);
            AssertAnswer("Q2", "Caption2", "Text", "Answer2", actualRegistration.Answers);
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
