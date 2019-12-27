using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to schedule a new event", SoThat = "People can meet up")]
    public class CreateEvent_Success : BaseTest
    {
        private ViewModels.CreateEvent _event;
        private Group _group;

        public CreateEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private async Task GivenIAmANormalUser()
        {
            await IAmANormalUser();
        }

        private async Task GivenIAmAnAdminstratorForTheGroup()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_group, User);

            var collection = GroupsCollection();

            await collection.InsertOneAsync(_group);
        }

        private async Task WhenICallTheCreateEventApi()
        {
            _event = Data.Create<ViewModels.CreateEvent>(e =>
            {
                e.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                e.EndTimeUTC = DateTime.UtcNow.AddDays(2);
                e.Questions = new List<ViewModels.Question>
                {
                    new ViewModels.Question { Code = "Question1", Caption = "Caption1", QuestionType = "Text" },
                    new ViewModels.Question { Code = "Question2", Caption = "Caption2", QuestionType = "Text" },
                };
            });

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/events", _event);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, Response.StatusCode);
        }

        private void ThenTheLocationOfTheNewEventIsReturned()
        {
            var location = Response.Headers.Location.ToString();
            Assert.Equal($"http://localhost/api/groups/{_group.Slug}/events/{_event.Slug}", location);
        }

        private async Task AndANewEventIsAddedToTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Slug == _event.Slug).SingleAsync();

            Assert.Equal(_event.Slug, actualEvent.Slug);
            Assert.Equal(_event.MaximumNumberOfAttendees, actualEvent.MaximumNumberOfAttendees);
            Assert.Equal(_event.Title, actualEvent.Title);
            Assert.Equal(_event.Description, actualEvent.Description);
            Assert.Equal(_event.GuestsAllowed, actualEvent.GuestsAllowed);
            Assert.Equal(_event.FoodProvided, actualEvent.FoodProvided);
            AssertDateTime(_event.StartTimeUTC, actualEvent.StartTimeUTC);
            AssertDateTime(_event.EndTimeUTC, actualEvent.EndTimeUTC);
            Assert.Equal(_event.Location, actualEvent.Location);
            Assert.Equal(_group.Id, actualEvent.GroupId);
            Assert.Equal(_group.Slug, actualEvent.GroupSlug);
            Assert.Equal(_group.Name, actualEvent.GroupName);

            Assert.Equal(2, actualEvent.Questions.Count);
            AssertQuestion("Question1", "Caption1", actualEvent.Questions);
            AssertQuestion("Question2", "Caption2", actualEvent.Questions);
        }

        private void AssertQuestion(string expectedCode, string expectedCaption, List<Question> questions)
        {
            var actualQuestion = questions.Single(q => q.Code == expectedCode);
            Assert.Equal(expectedCaption, actualQuestion.Caption);
            Assert.Equal(QuestionType.Text, actualQuestion.QuestionType);
        }
    }
}
