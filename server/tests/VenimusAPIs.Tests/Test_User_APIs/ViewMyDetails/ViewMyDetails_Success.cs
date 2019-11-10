using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.ViewMyDetails
{
    [Story(AsA = "User", IWant = "To be able to view my profile details", SoThat = "I can check they are upto date")]
    public class ViewMyDetails_Success : BaseTest
    {
        public ViewMyDetails_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/user");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenMyDetailsAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var actualDetails = JsonSerializer.Deserialize<ViewModels.ViewMyDetails>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(User.Bio, actualDetails.Bio);
            Assert.Equal(User.EmailAddress, actualDetails.EmailAddress);
            Assert.Equal(User.Pronoun, actualDetails.Pronoun);
            Assert.Equal(User.DisplayName, actualDetails.DisplayName);
            Assert.Equal(User.Fullname, actualDetails.Fullname);
            Assert.Equal(User.ProfilePicture, Convert.FromBase64String(actualDetails.ProfilePictureAsBase64));
        }
    }
}
