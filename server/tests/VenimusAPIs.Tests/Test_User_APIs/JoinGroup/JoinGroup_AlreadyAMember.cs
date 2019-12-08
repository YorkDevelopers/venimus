using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.JoinGroup
{
    [Story(AsA = "User", IWant = "To be able to join existing groups", SoThat = "I can join the community")]
    public class JoinGroup_AlreadyAMember : BaseTest
    {
        private ViewModels.JoinGroup _group;
        private Group _existingGroup;
        private GroupMember _existingMemberDetails;

        public JoinGroup_AlreadyAMember(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupAlreadyExistsAndIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            _existingMemberDetails = Data.AddGroupMember(_existingGroup, User);

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            _group = Data.Create<ViewModels.JoinGroup>();
            _group.GroupSlug = _existingGroup.Slug;

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/User/Groups", _group);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, Response.StatusCode);
        }

        private void ThenThePathToGroupIsReturned()
        {
            Assert.Equal($"http://localhost/api/User/Groups/{_existingGroup.Slug}", Response.Headers.Location.ToString());
        }

        private async Task ThenTheUserIsStillAMemberOfTheGroup()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Single(actualGroup.Members);
            var newMember = actualGroup.Members[0];
            Assert.Equal(_existingMemberDetails.UserId, newMember.UserId);
            Assert.Equal(_existingMemberDetails.Bio, newMember.Bio);
            Assert.Equal(_existingMemberDetails.DisplayName, newMember.DisplayName);
            Assert.Equal(_existingMemberDetails.EmailAddress, newMember.EmailAddress);
            Assert.Equal(_existingMemberDetails.Fullname, newMember.Fullname);
            Assert.Equal(_existingMemberDetails.Pronoun, newMember.Pronoun);
            Assert.Equal(_existingMemberDetails.IsAdministrator, newMember.IsAdministrator);
        }
    }
}
