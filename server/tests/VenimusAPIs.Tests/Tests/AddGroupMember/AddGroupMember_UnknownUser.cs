using System.Threading.Tasks;
using MongoDB.Bson;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AddGroupMember
{
    [Story(AsA = "Administrator", IWant = "to be able to add members to a group", SoThat = "They can belong to the community")]
    public class AddGroupMember_UnknownUser : BaseTest
    {
        private Group _existingGroup;

        public AddGroupMember_UnknownUser(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmAUser() => IAmASystemAdministrator();

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            var id = new ObjectId();
            var data = new ViewModels.AddGroupMember
            {
                Slug = id.ToString(),
                IsAdministrator = false,
            };

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_existingGroup.Slug}/Members", data);
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
