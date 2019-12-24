using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AddGroupMember
{
    [Story(AsA = "Administrator", IWant = "to be able to add members to a group", SoThat = "They can belong to the community")]
    public class AddGroupMember_UnknownGroup : BaseTest
    {
        private User _userToAdd;
        private bool _userIsAdministrator;

        public AddGroupMember_UnknownGroup(Fixture fixture) : base(fixture)
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

        private Task GivenIAmAUser() => IAmASystemAdministrator();

        private async Task GivenTheUserToAdd()
        {
            _userToAdd = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertOneAsync(_userToAdd);
        }

        private async Task WhenICallTheApiForAMadeUpGroup()
        {
            var data = new ViewModels.AddGroupMember
            {
                Slug = _userToAdd.Id.ToString(),
                IsAdministrator = _userIsAdministrator,
            };

            Response = await Fixture.APIClient.PostAsJsonAsync($"api/Groups/MADEUP/Members", data);
        }

        private void ThenANotFoundResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, Response.StatusCode);
        }
    }
}
