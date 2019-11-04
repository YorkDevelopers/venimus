using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to create new groups", SoThat = "People can build communities")]
    public class CreateGroup_Success : BaseTest
    {
        private string _token;
        private ViewModels.CreateGroup _group;

        public CreateGroup_Success(Fixture fixture) : base(fixture)
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

        private async Task WhenICallTheCreateGroupApi()
        {
            _group = Data.Create<ViewModels.CreateGroup>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _group.LogoInBase64 = Convert.ToBase64String(logo);

            Fixture.APIClient.SetBearerToken(_token);
            Response = await Fixture.APIClient.PostAsJsonAsync("api/Groups", _group);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, Response.StatusCode);
        }

        private async Task ThenANewGroupIsAddedToTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Slug == _group.Slug).SingleAsync();

            Assert.Equal(_group.Name, actualGroup.Name);
            Assert.Equal(_group.Description, actualGroup.Description);
            Assert.Equal(_group.SlackChannelName, actualGroup.SlackChannelName);
            Assert.Equal(_group.LogoInBase64, Convert.ToBase64String(actualGroup.Logo));
        }

        private void ThenTheLocationOfTheNewGroupIsReturned()
        {
            var location = Response.Headers.Location;
            var actualGroupName = location.Segments.Last();

            Assert.Equal(_group.Name, actualGroupName);
        }
    }
}
