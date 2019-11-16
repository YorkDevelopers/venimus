using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using VenimusAPIs.ViewModels;
using Xunit;

namespace VenimusAPIs.Tests.ViewGroupMembers
{
    [Story(AsA = "User", IWant = "to be able to view the other members of a group", SoThat = "I can belong to the community")]
    public class ViewGroupMembers_Success : BaseTest
    {
        private Group _existingGroup;
        private User _otherUserInGroup1;
        private User _otherUserInGroup2;
        private User _otherUserInGroup3;
        private User _otherUserNotInGroup1;

        public ViewGroupMembers_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmAUser() => IAmANormalUser();

        private async Task GivenThereAreOtherUsers()
        {
            _otherUserInGroup1 = Data.Create<Models.User>();
            _otherUserInGroup2 = Data.Create<Models.User>();
            _otherUserInGroup3 = Data.Create<Models.User>();
            _otherUserNotInGroup1 = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertManyAsync(new[] { _otherUserInGroup1, _otherUserInGroup2, _otherUserInGroup3, _otherUserNotInGroup1 });
        }

        private async Task GivenIAndSomeOthersBelongToTheGroup()
        {
            _existingGroup = Data.Create<Models.Group>(g =>
            {
                Data.AddApprovedGroupMember(g, User);
                Data.AddGroupMember(g, _otherUserInGroup1);
                Data.AddApprovedGroupMember(g, _otherUserInGroup2);
                Data.AddGroupAdministrator(g, _otherUserInGroup3);
            });

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task WhenICallTheApi()
        {
            Response = await Fixture.APIClient.GetAsync($"api/Groups/{_existingGroup.Slug}/Members");
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, Response.StatusCode);
        }

        private async Task ThenTheMembersAreReturned()
        {
            var json = await Response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var actualMembers = JsonSerializer.Deserialize<ViewModels.ListGroupMembers[]>(json, options);

            Assert.Equal(4, actualMembers.Length);

            AssertMember(User, actualMembers, false, true);
            AssertMember(_otherUserInGroup1, actualMembers, false, false);
            AssertMember(_otherUserInGroup2, actualMembers, false, true);
            AssertMember(_otherUserInGroup3, actualMembers, true, true);
        }

        private void AssertMember(User user, ListGroupMembers[] actualMembers, bool isAdministrator, bool isApproved)
        {
            var actualMember = actualMembers.Single(m => m.Slug == user.Id.ToString());

            Assert.Equal(user.DisplayName, actualMember.DisplayName);
            Assert.Equal(user.EmailAddress, actualMember.EmailAddress);
            Assert.Equal(user.Fullname, actualMember.Fullname);
            Assert.Equal(user.Bio, actualMember.Bio);
            Assert.Equal(user.Pronoun, actualMember.Pronoun);
            Assert.Equal(isAdministrator, actualMember.IsAdministrator);
            Assert.Equal(isApproved, actualMember.IsApproved);

            // Assert.Equal(user.ProfilePicture, Convert.FromBase64String(actualMember.ProfilePictureInBase64));
        }
    }
}
