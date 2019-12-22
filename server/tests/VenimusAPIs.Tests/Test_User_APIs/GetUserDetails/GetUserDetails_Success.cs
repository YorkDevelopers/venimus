using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Test_User_APIs.GetUserDetails
{
    public class GetUserDetails_Success : BaseTest
    {
        private User _user;

        public GetUserDetails_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmANormalUser() => IAmANormalUser();

        private async Task GivenTheUser()
        {
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertOneAsync(_user);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Users?DisplayName={_user.DisplayName}");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheDetailsOfTheUserAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualUser = JsonSerializer.Deserialize<ViewModels.GetUser>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(_user.DisplayName, actualUser.DisplayName);
            Assert.Equal(_user.EmailAddress, actualUser.EmailAddress);
            Assert.Equal(_user.Fullname, actualUser.Fullname);
            Assert.Equal(_user.Id.ToString(), actualUser.Slug);
            Assert.Equal(_user.Pronoun, actualUser.Pronoun);
            Assert.Equal(_user.Bio, actualUser.Bio);
            Assert.Equal($"http://localhost/api/users/{_user.Id}/profilepicture", actualUser.ProfileURL.ToString());
        }
    }
}
