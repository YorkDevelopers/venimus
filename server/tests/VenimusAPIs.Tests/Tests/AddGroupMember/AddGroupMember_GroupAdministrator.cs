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
    public class AddGroupMember_GroupAdministrator : BaseTest
    {
        private Group _existingGroup;
        private User _userToAdd;
        private bool _userIsAdministrator;

        public AddGroupMember_GroupAdministrator(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            _userIsAdministrator = false;

            this.WithExamples(new ExampleTable("_userIsAdministrator")
            {
                { false },
                { true },
            }).BDDfy();
        }

        private Task GivenIAmAUser() => IAmANormalUser();

        private async Task GivenTheUserToAdd()
        {
            _userToAdd = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertOneAsync(_userToAdd);
        }

        private async Task GivenAGroupAlreadyExistsAndIAmAnAdministrator()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupAdministrator(_existingGroup, User);

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            var data = new ViewModels.AddGroupMember
            {
                Slug = _userToAdd.Id.ToString(),
                IsAdministrator = _userIsAdministrator,
            };

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_existingGroup.Slug}/Members", data);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheUserIsNowAMemberOfTheGroup()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            var newMember = actualGroup.Members.Single(member => member.UserId == _userToAdd.Id);
            Assert.Equal(_userToAdd.Id, newMember.UserId);
            Assert.Equal(_userToAdd.Bio, newMember.Bio);
            Assert.Equal(_userToAdd.DisplayName, newMember.DisplayName);
            Assert.Equal(_userToAdd.EmailAddress, newMember.EmailAddress);
            Assert.Equal(_userToAdd.Fullname, newMember.Fullname);
            Assert.Equal(_userToAdd.Pronoun, newMember.Pronoun);
            Assert.Equal(_userIsAdministrator, newMember.IsAdministrator);
            Assert.Equal(_userToAdd.IsApproved, newMember.IsUserApproved);
        }
    }
}
