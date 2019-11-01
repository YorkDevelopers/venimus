using System;
using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.Models;

namespace VenimusAPIs.Tests.CreateGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to create new groups", SoThat = "People can build communities")]
    public class CreateGroup_DuplicateSlug : BaseTest
    {
        private HttpResponseMessage _response;
        private string _token;
        private ViewModels.CreateGroup _group;
        private Group _existingGroup;

        public CreateGroup_DuplicateSlug(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private async Task GivenIAmASystemAdministrator()
        {
            _token = await Fixture.GetTokenForSystemAdministrator();
        }

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheCreateGroupApiWithTheSameSlug()
        {
            _group = Data.Create<ViewModels.CreateGroup>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _group.LogoInBase64 = Convert.ToBase64String(logo);

            _group.Slug = _existingGroup.Slug;

            Fixture.APIClient.SetBearerToken(_token);
            _response = await Fixture.APIClient.PostAsJsonAsync("api/Groups", _group);
        }

        private async Task ThenABadRequestResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, _response.StatusCode);

            var json = await _response.Content.ReadAsStringAsync();
            var validationProblemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(json, new JsonSerializerOptions { IgnoreReadOnlyProperties = true });
            Assert.Equal("A group using this slug already exists", validationProblemDetails.Errors["Slug"].GetValue(0));
        }

        private async Task ThenTheDuplicateGroupIsNotAddedToTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroups = await groups.Find(u => u.Slug == _group.Slug).ToListAsync();
            Assert.Single(actualGroups);
        }
    }
}
