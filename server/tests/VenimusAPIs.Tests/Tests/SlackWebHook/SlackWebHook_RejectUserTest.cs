using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Services;
using VenimusAPIs.Services.SlackModels;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.Tests.SlackWebHook
{
    [Story(AsA = "Administrator", IWant = "To be able to approve user requests", SoThat = "They can join our community")]
    public class SlackWebHook_RejectUserTest : BaseTest
    {
        private User _userToAdd;
        private Interaction _interaction;
        private Group _existingGroup;
        private GroupEvent _existingEvent;

        public SlackWebHook_RejectUserTest(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Execute()
        {
            await ResetDatabase();
            this.BDDfy();
        }

        private async Task GivenAnUnApprovedUserExists()
        {
            _userToAdd = Data.Create<Models.User>(u => u.IsRejected = false);
            var collection = UsersCollection();
            await collection.InsertOneAsync(_userToAdd);
        }

        private async Task GivenTheUserIsAMemberOfAGroup()
        {
            _existingGroup = Data.Create<Models.Group>(g =>
            {
                Data.AddGroupMember(g, _userToAdd);
            });

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenTheUserIsAttendingAnEvent()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                Data.AddEventAttendee(evt, _userToAdd);
            });

            var events = EventsCollection();
            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenTheWebHookIsCalled()
        {
            _interaction = new Interaction
            {
                Actions = new Action[1]
                {
                    new Action
                    {
                         ActionID = SlackActionTypes.Reject,
                         Value = _userToAdd.Id.ToString(),
                    },
                },
                User = new InteractionUser
                {
                    UserName = "username_in_slack",
                },
                ResponseURL = new System.Uri("https://slack.com/12345"),
            };
            var json = JsonConvert.SerializeObject(_interaction);

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("payload", json));
            var content = new FormUrlEncodedContent(keyValues);

            Response = await Fixture.APIClient.PostAsync($"public/SlackWebHook", content);

            Response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheUserIsRejected()
        {
            var users = UsersCollection();
            var actualUser = await users.Find(u => u.Id == _userToAdd.Id).SingleAsync();

            Assert.False(actualUser.IsApproved);
            Assert.True(actualUser.IsRejected);
            Assert.Equal("username_in_slack", actualUser.ApprovedorRejectedBy);

            Assert.True(actualUser.ApprovedorRejectedOnUtc < System.DateTime.UtcNow);
            Assert.True(actualUser.ApprovedorRejectedOnUtc > System.DateTime.UtcNow.AddSeconds(-2));
        }

        private async Task ThenTheMessageInSlackIsUpdated()
        {
            var lastRequest = Fixture.MockSlack.LastRequest;
            var json = await lastRequest.Content.ReadAsStringAsync();

            Assert.Equal(_interaction.ResponseURL.ToString(), lastRequest.RequestUri.OriginalString);
            Assert.Contains(@"""replace_original"":true", json);
            Assert.Contains(_userToAdd.Fullname, json);
            Assert.Contains("username_in_slack", json);
            Assert.Contains(_userToAdd.EmailAddress, json);
            Assert.Contains(_userToAdd.Bio, json);
            Assert.Contains("rejected", json);
        }

        private async Task ThenTheUsersGroupMembershipIsNotUpdated()
        {
            await WaitForServiceBus();

            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _existingGroup.Id).SingleAsync();

            Assert.Single(actualGroup.Members);
            var actualMember = actualGroup.Members[0];
            Assert.False(actualMember.IsUserApproved);
        }

        private async Task ThenTheUsersEventRegistrationIsNotUpdated()
        {
            await WaitForServiceBus();

            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Single(actualEvent.Members);
            var actualMember = actualEvent.Members[0];
            Assert.False(actualMember.IsUserApproved);
        }
    }
}
