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
    public class UpdateGroup_NoChangeToLogo : BaseTest
    {
        private ViewModels.UpdateGroup _amendedGroup;
        private Group _existingGroup;

        public UpdateGroup_NoChangeToLogo(Fixture fixture) : base(fixture)
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

        private async Task WhenICallTheEditGroupApi()
        {
            _amendedGroup = Data.Create<ViewModels.UpdateGroup>();
            _amendedGroup.Slug = _existingGroup.Slug;       // Fix for 
            _amendedGroup.Name = _existingGroup.Name;       // this test
            _amendedGroup.LogoInBase64 = null;

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_existingGroup.Slug}", _amendedGroup);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenTheGroupDetailsAreUpdatedButTheLogoIsNot()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Equal(_amendedGroup.Slug, actualGroup.Slug);
            Assert.Equal(_amendedGroup.Name, actualGroup.Name);
            Assert.Equal(_amendedGroup.StrapLine, actualGroup.StrapLine);
            Assert.Equal(_amendedGroup.Description, actualGroup.Description);
            Assert.Equal(_amendedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(_existingGroup.Logo, actualGroup.Logo);
        }
    }
}
