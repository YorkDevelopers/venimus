using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.JoinGroup
{
    [Story(AsA = "User", IWant = "To be able to join existing groups", SoThat = "I can join the community")]
    public class JoinGroup_Success : BaseTest
    {
        private ViewModels.JoinGroup _group;
        private Group _existingGroup;

        public JoinGroup_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

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
            Assert.Equal($"http://localhost/api/Groups/{_existingGroup.Slug}", Response.Headers.Location.ToString());
        }

        private async Task ThenTheUserIsNowAMemberOfTheGroup()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Single(actualGroup.Members);
            var newMember = actualGroup.Members[0];
            Assert.Equal(User.Id, newMember.UserId);
            Assert.Equal(User.Bio, newMember.Bio);
            Assert.Equal(User.DisplayName, newMember.DisplayName);
            Assert.Equal(User.EmailAddress, newMember.EmailAddress);
            Assert.Equal(User.Fullname, newMember.Fullname);
            Assert.Equal(User.Pronoun, newMember.Pronoun);
            Assert.False(newMember.IsAdministrator);
            Assert.False(newMember.IsUserApproved);
        }
    }
}
