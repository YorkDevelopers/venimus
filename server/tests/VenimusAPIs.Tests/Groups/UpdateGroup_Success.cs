using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to update existing groups", SoThat = "People can build communities")]
    public class UpdateGroup_Success : BaseTest
    {
        /*
         * Group does not exist
         * No permission
         * Group name not unique
         */

        private HttpResponseMessage _response;
        private string _token;
        private ViewModels.UpdateGroup _amendedGroup;
        private Group _existingGroup;

        public UpdateGroup_Success(Fixture fixture) : base(fixture)
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

        private async Task WhenICallTheEditGroupApi()
        {
            _amendedGroup = Data.Create<ViewModels.UpdateGroup>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _amendedGroup.LogoInBase64 = Convert.ToBase64String(logo);

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_existingGroup.Slug}", _amendedGroup);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, _response.StatusCode);
        }

        private async Task ThenTheGroupDetailsAreUpdated()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Equal(_amendedGroup.Slug, actualGroup.Slug);
            Assert.Equal(_amendedGroup.Name, actualGroup.Name);
            Assert.Equal(_amendedGroup.Description, actualGroup.Description);
            Assert.Equal(_amendedGroup.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(_amendedGroup.LogoInBase64, Convert.ToBase64String(actualGroup.Logo));
        }
    }
}
