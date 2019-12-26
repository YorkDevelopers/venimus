using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateEvent
{
    [Story(AsA = "GroupAdministrator", IWant = "To be able to schedule a new event", SoThat = "People can meet up")]
    public class CreateEvent_DuplicateQuestion : BaseTest
    {
        private ViewModels.CreateEvent _event;
        private Group _group;

        public CreateEvent_DuplicateQuestion(Fixture fixture) : base(fixture)
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
                e.Questions = new List<Question>
                {
                    new Question { Code = "Question1", Caption = "Caption1", QuestionType = QuestionType.Text },
                    new Question { Code = "Question1", Caption = "Caption2", QuestionType = QuestionType.Text },
                };
            });

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/events", _event);
        }

        private async Task ThenABadRequestResponseIsReturned()
        {
            await AssertBadRequest("Code", "Two questions cannot be created with the same code.");
        }

        private async Task AndANewEventIsNotAddedToTheDatabase()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Slug == _event.Slug).SingleOrDefaultAsync();
            Assert.Null(actualEvent);
        }
    }
}
