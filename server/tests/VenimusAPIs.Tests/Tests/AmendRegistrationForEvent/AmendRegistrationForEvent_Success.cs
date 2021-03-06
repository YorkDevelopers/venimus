using MongoDB.Driver;
using System.Threading.Tasks;
using TestStack.BDDfy;
using VenimusAPIs.Models;
using VenimusAPIs.Tests.Extensions;
using VenimusAPIs.Tests.Infrastucture;
using Xunit;

namespace VenimusAPIs.Tests.AmendRegistrationForEvent
{
    [Story(AsA = "User", IWant = "To be able to sign up to events", SoThat = "I can attend them")]
    public class AmendRegistrationForEvent_Success : BaseTest
    {
        private Group _existingGroup;
        private GroupEvent _existingEvent;
        private ViewModels.RegisterForEvent _amendedDetails;

        public AmendRegistrationForEvent_Success(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private Task GivenIAmUser() => IAmANormalUser();

        private async Task GivenAGroupExistsOfWhichIAmAMember()
        {
            _existingGroup = Data.Create<Models.Group>();
            Data.AddGroupMember(_existingGroup, User);

            var groups = GroupsCollection();

            await groups.InsertOneAsync(_existingGroup);
        }

        private async Task GivenAnEventExistsForThatGroupAndIAmGoing()
        {
            _existingEvent = Data.CreateEvent(_existingGroup, evt =>
            {
                Data.AddEventAttendee(evt, User, numberOfGuests: 5);
            });

            var events = EventsCollection();

            await events.InsertOneAsync(_existingEvent);
        }

        private async Task WhenICallTheApi()
        {
            _amendedDetails = Data.Create<ViewModels.RegisterForEvent>();
            _amendedDetails.AddAnswer("NumberOfGuests", "6");
            _amendedDetails.AddAnswer("DietaryRequirements", "ExampleDietaryRequirements");
            _amendedDetails.AddAnswer("MessageToOrganiser", "ExampleMessageToOrganiser");

            Response = await Fixture.APIClient.PutAsJsonAsync($"api/user/groups/{_existingGroup.Slug}/Events/{_existingEvent.Slug}", _amendedDetails);
        }

        private void ThenASuccessResponseIsReturned()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, Response.StatusCode);
        }

        private async Task ThenTheUsersRegistrationIsUpdated()
        {
            var events = EventsCollection();
            var actualEvent = await events.Find(u => u.Id == _existingEvent.Id).SingleAsync();

            Assert.Single(actualEvent.Members);

            var member = actualEvent.Members[0];
            Assert.Equal(User.Id.ToString(), member.UserId.ToString());
            Assert.Equal("ExampleDietaryRequirements", member.DietaryRequirements);
            Assert.Equal(6, member.NumberOfGuests);
            Assert.True(member.SignedUp);
        }
    }
}
