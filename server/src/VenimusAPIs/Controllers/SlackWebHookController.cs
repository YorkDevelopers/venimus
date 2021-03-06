﻿using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using VenimusAPIs.Mongo;
using VenimusAPIs.ServiceBus;
using VenimusAPIs.Services;
using VenimusAPIs.Services.SlackModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public partial class SlackWebHookController : BaseController
    {
        private readonly UserStore _userStore;
        private readonly Slack _slack;
        private readonly EventPublisher _eventPublisher;
        private readonly SlackMessages _slackMessages;

        public SlackWebHookController(UserStore userStore, Slack slack, EventPublisher eventPublisher, SlackMessages slackMessages)
        {
            _userStore = userStore;
            _slack = slack;
            _eventPublisher = eventPublisher;
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
            var userApprovalID = new System.Guid(action.Value);
            var user = await _userStore.GetUserByApprovalId(userApprovalID).ConfigureAwait(false);
            if (user == null)
            {
                throw new System.Exception(Resources.ResourceMessages.INTERNALERROR_USER_DOES_NOT_EXIST);
            }

            user.IsApproved = true;
            user.IsRejected = false;
            user.ApprovedorRejectedBy = interaction.User.UserName;
            user.ApprovedorRejectedOnUtc = System.DateTime.UtcNow;
            await _userStore.UpdateUser(user).ConfigureAwait(false);

            var sendTo = interaction.ResponseURL;

            var message = _slackMessages.BuildApprovedRequestMessage(user);
            await _slack.SendAdvancedMessage(message, sendTo).ConfigureAwait(false);

            await _eventPublisher.UserChanged(user).ConfigureAwait(false);
        }

        private async Task RejectUser(Interaction interaction, Action action)
        {
            var userApprovalID = new System.Guid(action.Value);
            var user = await _userStore.GetUserByApprovalId(userApprovalID).ConfigureAwait(false);
            if (user == null)
            {
                throw new System.Exception(Resources.ResourceMessages.INTERNALERROR_USER_DOES_NOT_EXIST);
            }

            user.IsApproved = false;
            user.IsRejected = true;
            user.ApprovedorRejectedBy = interaction.User.UserName;
            user.ApprovedorRejectedOnUtc = System.DateTime.UtcNow;

            await _userStore.UpdateUser(user).ConfigureAwait(false);

            var sendTo = interaction.ResponseURL;

            var message = _slackMessages.BuildRejectedRequestMessage(user);
            await _slack.SendAdvancedMessage(message, sendTo).ConfigureAwait(false);

            await _eventPublisher.UserChanged(user).ConfigureAwait(false);
        }
    }
}
