using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to update existing groups", SoThat = "People can build communities")]
    public class UpdateGroup_DuplicateSlug : BaseTest
    {
        private string _token;
        private ViewModels.UpdateGroup _amendedGroup;
        private Group _existingGroup;
        private Group _anotherGroup;

        public UpdateGroup_DuplicateSlug(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmASystemAdministrator()
        {
            _token = await Fixture.GetTokenForSystemAdministrator();
        }

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnotherGroupAlreadyExists()
        {
            _anotherGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_anotherGroup);
        }

        private async Task WhenICallTheEditGroupApiWithTheSameSlug()
        {
            _amendedGroup = Data.Create<ViewModels.UpdateGroup>();
            _amendedGroup.LogoInBase64 = Convert.ToBase64String(_existingGroup.Logo);
            _amendedGroup.Slug = _anotherGroup.Slug;

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_existingGroup.Slug}", _amendedGroup);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("Slug", "A group using this slug already exists");
        }

        private async Task ThenTheGroupDetailsAreNotUpdated()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Equal(_existingGroup.Slug, actualGroup.Slug);
            Assert.Equal(_existingGroup.Name, actualGroup.Name);
            Assert.Equal(_existingGroup.Description, actualGroup.Description);
            Assert.Equal(_existingGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(_existingGroup.Logo, actualGroup.Logo);
        }
    }
}
