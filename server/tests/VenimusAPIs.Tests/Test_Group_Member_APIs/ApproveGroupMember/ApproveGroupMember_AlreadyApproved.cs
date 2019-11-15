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
    public class ApproveGroupMember_AlreadyApproved : BaseTest
    {
        private Group _group;
        private User _newMember;

        public ApproveGroupMember_AlreadyApproved(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmAUser() => IAmANormalUser();

        private void GivenIAmAnAdministratorOfAGroup()
        {
            _group = Data.Create<Models.Group>(g =>
            {
                Data.AddGroupAdministrator(g, User);
            });
        }

        private async Task GivenThereIsAnExistingMemberOfTheGroup()
        {
            _newMember = Data.Create<Models.User>();

            var collection = UsersCollection();
            await collection.InsertOneAsync(_newMember);

            Data.AddGroupMember(_group, _newMember, isApproved: true);

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_group);
        }

        private async Task WhenICallTheApi()
        {
            var data = new ApproveMember { UserSlug = _newMember.Id.ToString() };
            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/{_group.Slug}/ApprovedMembers", data);
        }

        private async Task ThenABadRquestResponseIsReturned()
        {
            await AssertBadRequestDetail("The user's membership of this group has already been approved.");
        }
    }
}
