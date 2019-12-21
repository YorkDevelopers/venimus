using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AddGroupMember
{
    [Story(AsA = "Administrator", IWant = "to be able to add members to a group", SoThat = "They can belong to the community")]
    public class AddGroupMember_NoPermission : BaseTest
    {
        private Group _existingGroup;
        private User _userToAdd;

        public AddGroupMember_NoPermission(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmAUser() => IAmANormalUser();

        private async Task GivenTheUserToAdd()
        {
            _userToAdd = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertOneAsync(_userToAdd);
        }

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            var data = new ViewModels.AddGroupMember
            {
                Slug = _userToAdd.Id.ToString(),
                IsAdministrator = false,
            };

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_existingGroup.Slug}/Members", data);
        }

        private void ThenA403ResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }

        private async Task ThenTheUserIsNotAMemberOfTheGroup()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleAsync();
            var newMember = actualGroup.Members.SingleOrDefault(member => member.UserId == _userToAdd.Id);

            Assert.Null(newMember);
        }
    }
}
