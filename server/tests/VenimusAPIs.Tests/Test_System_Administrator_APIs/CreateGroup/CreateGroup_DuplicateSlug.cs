using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.CreateGroup
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to create new groups", SoThat = "People can build communities")]
    public class CreateGroup_DuplicateSlug : BaseTest
    {
        private string Culture = string.Empty;
        private string ExpectedMessage = string.Empty; 
        private ViewModels.CreateGroup _group;
        private Group _existingGroup;

        public CreateGroup_DuplicateSlug(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("Culture", "ExpectedMessage")
            {
                { Cultures.Normal, "A group using this slug already exists" },
                { Cultures.Test, "'€'A group using this slug already exists" },
            }).BDDfy();
        }

        private Task GivenIAmASystemAdministrator() => IAmASystemAdministrator();

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

            Fixture.APIClient.SetCulture(Culture);
            Response = await Fixture.APIClient.PostAsJsonAsync("api/Groups", _group);
        }

        private Task ThenABadRequestResponseIsReturned()
        {
            return AssertBadRequest("Slug", ExpectedMessage);
        }

        private async Task ThenTheDuplicateGroupIsNotAddedToTheDatabase()
        {
            var groups = GroupsCollection();
            var actualGroups = await groups.Find(u => u.Slug == _group.Slug).ToListAsync();
            Assert.Single(actualGroups);
        }
    }
}
