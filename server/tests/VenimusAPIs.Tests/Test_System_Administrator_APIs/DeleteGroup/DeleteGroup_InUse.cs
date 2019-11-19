using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.DeleteGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to delete unused groups", SoThat = "The database can be kept tidy")]
    public class DeleteGroup_InUse : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty; 
        private Group _existingGroup;
        private Event _event1;
        private Event _event2;

        public DeleteGroup_InUse(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "The group cannot be deleted as it has one or events.  Please mark the group as InActive instead." },
                { Cultures.Test, "'\u20AC'The group cannot be deleted as it has one or events.  Please mark the group as InActive instead." },
            }).BDDfy();
        }

        private Task GivenIAmASystemAdministrator() => IAmASystemAdministrator();

        private async Task GivenAGroupExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenEventsExistForTheGroup()
        {
            _event1 = Data.CreateEvent(_existingGroup);
            _event2 = Data.CreateEvent(_existingGroup);

            var events = EventsCollection();

            await events.InsertOneAsync(_event1);
            await events.InsertOneAsync(_event2);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.DeleteAsync($"api/Groups/{_existingGroup.Slug}");
        }

        private Task ThenABadRequestIsReturned()
        {
            return AssertBadRequestDetail(ExpectedMessage);
        }

        private async Task ThenTheGroupIsNotRemovedFromTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Slug == _existingGroup.Slug).SingleOrDefaultAsync();

            Assert.NotNull(actualGroup);
        }
    }
}
