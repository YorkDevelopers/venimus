using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewMyDetails
{
    [Story(AsA = "User", IWant = "To be able to view my profile details", SoThat = "I can check they are upto date")]
    public class ViewMyDetails_Success : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private string _uniqueID;
        private User _user;

        public ViewMyDetails_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIAmUser()
        {
            _uniqueID = Guid.NewGuid().ToString();
            _token = Fixture.GetTokenForNewUser(_uniqueID);
        }

        private async Task GivenAlreadyExistInTheDatabase()
        {
            _user = Data.Create<Models.User>();

            var collection = UsersCollection();

            _user.Identities = new List<string> { _uniqueID };

            await collection.InsertOneAsync(_user);
        }

        private async Task WhenICallTheApi()
        {
            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.GetAsync($"api/user");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        private async Task ThenMyDetailsAreReturned()
        {
            var json = await _response.Content.ReadAsStringAsync();
            var actualDetails = JsonSerializer.Deserialize<ViewModels.ViewMyDetails>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(_user.Bio, actualDetails.Bio);
            Assert.Equal(_user.EmailAddress, actualDetails.EmailAddress);
            Assert.Equal(_user.Pronoun, actualDetails.Pronoun);
            Assert.Equal(_user.DisplayName, actualDetails.DisplayName);
            Assert.Equal(_user.Fullname, actualDetails.Fullname);
            Assert.Equal(_user.ProfilePicture, Convert.FromBase64String(actualDetails.ProfilePictureAsBase64));
        }
    }
}
