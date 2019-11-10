using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.LeaveGroup
{
    [Story(AsA = "User", IWant = "To be able to leave groups that I'm a member of", SoThat = "I can leave the community")]
    public class LeaveGroup_NotAMember : BaseTest
    {
        private Group _existingGroup;

        public LeaveGroup_NotAMember(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupWhichIDoNotBelongToExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.DeleteAsync($"api/User/Groups/{_existingGroup.Slug}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenTheUserIsNotAMemberOfTheGroup()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleOrDefaultAsync();

            Assert.Null(actualGroup.Members);
        }
    }
}
