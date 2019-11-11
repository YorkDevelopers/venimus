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
    public class ViewGroupMembers_SysAdmin_Success : BaseTest
    {
        private Group _existingGroup;
        private User _otherUserInGroup1;
        private User _otherUserInGroup2;
        private User _otherUserInGroup3;
        private User _otherUserNotInGroup1;

        public ViewGroupMembers_SysAdmin_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmASystemAdministrator() => IAmASystemAdministrator();

        private async Task GivenThereAreOtherUsers()
        {
            _otherUserInGroup1 = Data.Create<Models.User>();
            _otherUserInGroup2 = Data.Create<Models.User>();
            _otherUserInGroup3 = Data.Create<Models.User>();
            _otherUserNotInGroup1 = Data.Create<Models.User>();

            var collection = UsersCollection();

            await collection.InsertManyAsync(new[] { _otherUserInGroup1, _otherUserInGroup2, _otherUserInGroup3, _otherUserNotInGroup1 });
        }

        private async Task GivenIDoNotBelongToTheGroupButOthersDo()
        {
            _existingGroup = Data.Create<Models.Group>();
            _existingGroup.Members = new List<Group.GroupMember>()
            {
                new Group.GroupMember { UserId = _otherUserInGroup1.Id },
                new Group.GroupMember { UserId = _otherUserInGroup2.Id },
                new Group.GroupMember { UserId = _otherUserInGroup3.Id },
            };

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

            Assert.Equal(3, actualMembers.Length);

            AssertMember(_otherUserInGroup1, actualMembers);
            AssertMember(_otherUserInGroup2, actualMembers);
            AssertMember(_otherUserInGroup3, actualMembers);
        }

        private void AssertMember(User user, ListGroupMembers[] actualMembers)
        {
            var actualMember = actualMembers.Single(m => m.Slug == user.Id.ToString());

            Assert.Equal(user.DisplayName, actualMember.DisplayName);
            Assert.Equal(user.EmailAddress, actualMember.EmailAddress);
            Assert.Equal(user.Fullname, actualMember.Fullname);
            Assert.Equal(user.Bio, actualMember.Bio);
            Assert.Equal(user.Pronoun, actualMember.Pronoun);
            Assert.Equal(user.ProfilePicture, Convert.FromBase64String(actualMember.ProfilePictureInBase64));
        }
    }
}
