using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Threading.Tasks;
using VenimusAPIs.Mongo;
using VenimusAPIs.Services;
using VenimusAPIs.Services.SlackModels;

namespace VenimusAPIs.PublicControllers
{
    [ApiController]
    public partial class SlackWebHookController : ControllerBase
    {
        private readonly UserStore _userStore;
        private readonly Slack _slack;
        private readonly IBus _bus;
        private readonly SlackMessages _slackMessages;

        public SlackWebHookController(UserStore userStore, Slack slack, IBus bus, SlackMessages slackMessages)
        {
            _userStore = userStore;
            _slack = slack;
            _bus = bus;
            _slackMessages = slackMessages;
        }

        [Route("public/SlackWebHook")]
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Post()
        {
            var data = Request.Form["payload"];
            var interaction = JsonConvert.DeserializeObject<Interaction>(data);
            var action = interaction.Actions[0];

            switch (action.ActionID)
            {
                case SlackActionTypes.Approve:
                    await ApproveUser(interaction, action).ConfigureAwait(false);
                    break;

                case SlackActionTypes.Reject:
                    await RejectUser(interaction, action).ConfigureAwait(false);
                    break;

                default:
                    break;
            }

            return Ok();
        }

        private async Task ApproveUser(Interaction interaction, Action action)
        {
            var userID = new ObjectId(action.Value);
            var user = await _userStore.GetUserById(userID).ConfigureAwait(false);
            user.IsApproved = true;
            user.IsRejected = false;
            user.ApprovedorRejectedBy = interaction.User.UserName;
            user.ApprovedorRejectedOnUtc = System.DateTime.UtcNow;
            await _userStore.UpdateUser(user).ConfigureAwait(false);

            var sendTo = interaction.ResponseURL;

            // TODO:
            user.Bio = "The user's bio will go here.";
            var message = _slackMessages.BuildApprovedRequestMessage(user);
            await _slack.SendAdvancedMessage(message, sendTo).ConfigureAwait(false);

            var userChangedMessage = new ServiceBusMessages.UserChangedMessage { UserId = user.Id.ToString() };
            await _bus.Publish(userChangedMessage).ConfigureAwait(false);
        }

        private async Task RejectUser(Interaction interaction, Action action)
        {
            var userID = new ObjectId(action.Value);
            var user = await _userStore.GetUserById(userID).ConfigureAwait(false);
            user.IsApproved = false;
            user.IsRejected = true;
            user.ApprovedorRejectedBy = interaction.User.UserName;
            user.ApprovedorRejectedOnUtc = System.DateTime.UtcNow;

            await _userStore.UpdateUser(user).ConfigureAwait(false);

            var sendTo = interaction.ResponseURL;

            // TODO:
            user.Bio = "The user's bio will go here.";
            var message = _slackMessages.BuildRejectedRequestMessage(user);
            await _slack.SendAdvancedMessage(message, sendTo).ConfigureAwait(false);

            var userChangedMessage = new ServiceBusMessages.UserChangedMessage { UserId = user.Id.ToString() };
            await _bus.Publish(userChangedMessage).ConfigureAwait(false);
        }
    }
}
