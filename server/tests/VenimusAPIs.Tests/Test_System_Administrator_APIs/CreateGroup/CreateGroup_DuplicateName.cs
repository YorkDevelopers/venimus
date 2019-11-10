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
    public class CreateGroup_DuplicateName : BaseTest
    {
        private ViewModels.CreateGroup _group;
        private Group _existingGroup;

        public CreateGroup_DuplicateName(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmASystemAdministrator() => IAmASystemAdministrator();

        private async Task GivenAGroupAlreadyExists()
        {
            _existingGroup = Data.Create<Models.Group>();

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheCreateGroupApiWithTheSameName()
        {
            _group = Data.Create<ViewModels.CreateGroup>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _group.LogoInBase64 = Convert.ToBase64String(logo);

            _group.Name = _existingGroup.Name;

            Response = await Fixture.APIClient.PostAsJsonAsync("api/Groups", _group);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("Name", "A group using this name already exists");
        }

        private async Task ThenTheDuplicateGroupIsNotAddedToTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroups = await groups.Find(u => u.Name == _group.Name).ToListAsync();
            Assert.Single(actualGroups);
        }
    }
}
