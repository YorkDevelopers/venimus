using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewMyRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class ViewMyRegistrationForEvent_NotSignedUp : BaseTest
    {
        private Group _existingGroup;
        private GroupEvent _existingEvent;

        public ViewMyRegistrationForEvent_NotSignedUp(Fixture fixture) : base(fixture)
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

        private async Task GivenAnEventExistsForThatGroupAndIAmNotGoing()
        {
            _existingEvent = Data.CreateEvent(_existingGroup);

            var events = EventsCollection();
            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}");
        }

        private async Task ThenTheDetailsOfTheRegistrationAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualRegistration = JsonSerializer.Deserialize<ViewMyEventRegistration>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.False(actualRegistration.Attending);
            Assert.Equal(_existingGroup.Name, actualRegistration.GroupName);
            Assert.Equal(_existingEvent.Title, actualRegistration.EventTitle);

            Assert.Equal(4, actualRegistration.Answers.Length);
            AssertAnswer("Q1", "Caption1", "Text", actualRegistration.Answers);
            AssertAnswer("Q2", "Caption2", "Text", actualRegistration.Answers);
            AssertAnswer("NumberOfGuests", "Number of Guests", "NumberOfGuests", actualRegistration.Answers);
            AssertAnswer("DietaryRequirements", "Any dietary requirements?", "DietaryRequirements", actualRegistration.Answers);
        }

        private void AssertAnswer(string expectedCode, string expectedCaption, string expectedQuestionType, ViewModels.Answer[] allAnswers)
        {
            var actualAnswer = allAnswers.Single(a => a.Code == expectedCode);
            Assert.Equal(expectedCaption, actualAnswer.Caption);
            Assert.Equal(expectedQuestionType, actualAnswer.QuestionType);
            Assert.Equal(string.Empty, actualAnswer.UsersAnswer);
        }
    }
}
