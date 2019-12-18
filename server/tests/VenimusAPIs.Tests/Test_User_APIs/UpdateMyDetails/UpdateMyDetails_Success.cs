using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.UpdateMyDetails
{
    [Story(AsA = "User", IWant = "To be able to update my profile details", SoThat = "I can ensure they are upto date")]
    public class UpdateMyDetails_Success : BaseTest
    {
        private ViewModels.UpdateMyDetails _amendedUser;
        private Group _group;
        private GroupEvent _event;

        public UpdateMyDetails_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupAlreadyExistsAndIAmAMember()
        {
            _group = Data.Create<Models.Group>();
            Data.AddGroupMember(_group, User);

            var groups = GroupsCollection();
            await groups.InsertOneAsync(_group);
        }

        private async Task GivenAnEventExistsAndIAmAlreadySignedUp()
        {
            _event = Data.CreateEvent(_group, e =>
            {
                Data.AddEventAttendee(e, User);
            });

            var events = EventsCollection();
            await events.InsertOneAsync(_event);
        }

        private async Task WhenICallTheApi()
        {
            _amendedUser = Data.Create<ViewModels.UpdateMyDetails>();
            var logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg");
            _amendedUser.ProfilePictureAsBase64 = Convert.ToBase64String(logo);

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user", _amendedUser);

            await WaitForServiceBus();
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenMyDetailsAreUpdatedInTheDatabase()
        {
            var users = UsersCollection();
            var actualUser = await users.Find(u => u.Id == User.Id).SingleAsync();

            Assert.Equal(_amendedUser.Bio, actualUser.Bio);
            Assert.Equal(_amendedUser.Pronoun, actualUser.Pronoun);
            Assert.Equal(_amendedUser.DisplayName, actualUser.DisplayName);
            Assert.Equal(_amendedUser.Fullname, actualUser.Fullname);
            Assert.Equal(_amendedUser.IsRegistered, actualUser.IsRegistered);
            Assert.Equal(_amendedUser.ProfilePictureAsBase64, Convert.ToBase64String(actualUser.ProfilePicture));
        }

        private async Task ThenMyDetailsAreUpdatedInAnyGroupsIBelongTo()
        {
            var groups = GroupsCollection();
            var actualGroup = await groups.Find(u => u.Id == _group.Id).SingleOrDefaultAsync();

            Assert.Single(actualGroup.Members);
            var membershipDetails = actualGroup.Members[0];
            Assert.Equal(_amendedUser.Bio, membershipDetails.Bio);
            Assert.Equal(_amendedUser.DisplayName, membershipDetails.DisplayName);
            Assert.Equal(_amendedUser.Fullname, membershipDetails.Fullname);
            Assert.Equal(_amendedUser.Pronoun, membershipDetails.Pronoun);
        }

        private async Task ThenMyDetailsAreUpdatedInAnyEventsIBelongTo()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _event.Id).SingleOrDefaultAsync();

            Assert.Single(actualEvent.Members);
            var membershipDetails = actualEvent.Members[0];
            Assert.Equal(_amendedUser.Bio, membershipDetails.Bio);
            Assert.Equal(_amendedUser.DisplayName, membershipDetails.DisplayName);
            Assert.Equal(_amendedUser.Fullname, membershipDetails.Fullname);
            Assert.Equal(_amendedUser.Pronoun, membershipDetails.Pronoun);
        }
    }
}