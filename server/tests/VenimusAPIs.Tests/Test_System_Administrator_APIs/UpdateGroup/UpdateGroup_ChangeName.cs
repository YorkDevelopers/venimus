using System;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to update existing groups", SoThat = "People can build communities")]
    public class UpdateGroup_ChangeName : BaseTest
    {
        private ViewModels.UpdateGroup _amendedGroup;
        private Group _existingGroup;
        private Event _event1;
        private Event _event2;

        public UpdateGroup_ChangeName(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmASystemAdministrator() => IAmASystemAdministrator();

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenSomeEventsForTheGroup()
        {
            _event1 = Data.CreateEvent(_existingGroup);
            _event2 = Data.CreateEvent(_existingGroup);

            var events = EventsCollection();

            await events.InsertOneAsync(_event1);
            await events.InsertOneAsync(_event2);
        }

        private async Task WhenICallTheEditGroupApi()
        {
            _amendedGroup = Data.Create<ViewModels.UpdateGroup>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _amendedGroup.LogoInBase64 = Convert.ToBase64String(logo);
            _amendedGroup.Slug = _existingGroup.Slug; // Fix slug for this test

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_existingGroup.Slug}", _amendedGroup);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenTheEventDetailsAreUpdated()
        {
            var events = EventsCollection();
            
            var actualEvent1 = await events.Find(u => u.Id == _event1.Id).SingleAsync();
            Assert.Equal(_amendedGroup.Slug, actualEvent1.GroupSlug);
            Assert.Equal(_amendedGroup.Name, actualEvent1.GroupName);

            var actualEvent2 = await events.Find(u => u.Id == _event2.Id).SingleAsync();
            Assert.Equal(_amendedGroup.Slug, actualEvent2.GroupSlug);
            Assert.Equal(_amendedGroup.Name, actualEvent2.GroupName);
        }
    }
}
