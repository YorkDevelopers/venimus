using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewGroupMembers.ApproveGroupMember
{
    [Story(AsA = "Group administrator", IWant = "to be able to approve new members to the group", SoThat = "Then can belong to the community")]
    public class ApproveGroupMember_NoPermission : BaseTest
    {
        private Group _group;
        private User _newMember;

        public ApproveGroupMember_NoPermission(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmAUser() => IAmANormalUser();

        private void GivenIAmNotAnAdministratorOfAGroup()
        {
            _group = Data.Create<Models.Group>(g =>
            {
                Data.AddGroupMember(g, User);
            });
        }

        private async Task GivenThereIsANewMemberOfTheGroup()
        {
            _newMember = Data.Create<Models.User>();

            var collection = UsersCollection();
            await collection.InsertOneAsync(_newMember);

            Data.AddGroupMember(_group, _newMember);

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_group);
        }

        private async Task WhenICallTheApi()
        {
            var data = new ApproveMember { UserSlug = _newMember.Id.ToString() };
            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/ApprovedMembers", data);
        }

        private void ThenAForbidResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, Response.StatusCode);
        }

        private async Task ThenTheNewMemberIsNotApproved()
        {
            var groups = GroupsCollection();

            var actualGroup = await groups.Find(g => g.Id == _group.Id).SingleAsync();
            var actualMember = actualGroup.Members.Single(m => m.UserId == _newMember.Id);

            Assert.False(actualMember.IsApproved);
        }
    }
}
