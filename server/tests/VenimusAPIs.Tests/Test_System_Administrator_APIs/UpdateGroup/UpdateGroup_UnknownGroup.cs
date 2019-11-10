using System;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to update existing groups", SoThat = "People can build communities")]
    public class UpdateGroup_UnknownGroup : BaseTest
    {
        private Group _existingGroup;

        public UpdateGroup_UnknownGroup(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmASystemAdministrator() => IAmASystemAdministrator();

        private void GivenThgeGroupDoesNotExists()
        {
            _existingGroup = Data.Create<Models.Group>();
        }

        private async Task WhenICallTheEditGroupApiWithTheSameName()
        {
            var amendedGroup = Data.Create<ViewModels.UpdateGroup>();
            amendedGroup.LogoInBase64 = Convert.ToBase64String(_existingGroup.Logo);

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/Groups/{_existingGroup.Slug}", amendedGroup);
        }

        private void ThenNotFoundIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
