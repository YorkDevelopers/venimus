using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Mongo;
using VenimusAPIs.Services;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class EventsAttendeesController : BaseController
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.UserStore _userStore;
        private readonly GroupStore _groupStore;
        private readonly URLBuilder _urlBuilder;

        public EventsAttendeesController(Mongo.EventStore eventStore, Mongo.UserStore userStore, Mongo.GroupStore groupStore, URLBuilder urlBuilder)
        {
            _eventStore = eventStore;
            _userStore = userStore;
            _groupStore = groupStore;
            _urlBuilder = urlBuilder;
        }

        /// <summary>
        ///     Allows you to retrieve the members of an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo/Events/Nov2019/Members
        ///
        /// </remarks>
        /// <returns>The ListEventAttendees view model</returns>
        /// <response code="200">Success</response>
        /// <response code="401">No Access.</response>
        /// <response code="403">No Permission.</response>
        /// <response code="404">Group does not exist.</response>
        [Authorize]
        [Route("api/Groups/{groupSlug}/Events/{eventSlug}/Members")]
        [HttpGet]
        [CallerMustBeApprovedGroupMember]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListEventAttendees[]>> Get([FromRoute, Slug]string groupSlug, [FromRoute, Slug]string eventSlug)
        {
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
            if (theEvent == null)
            {
                return NotFound();
            }

            var uniqueID = UniqueIDForCurrentUser;
            var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);
            if (group == null)
            {
                return NotFound();
            }

            var canViewDetails = false;
            var member = group.Members.SingleOrDefault(member => member.UserId == existingUser.Id);
            if (member != null)
            {
                canViewDetails = member.IsAdministrator;
            }

            return theEvent.Members.Select(attendee => new ListEventAttendees
            {
                Bio = attendee.Bio,
                DisplayName = attendee.DisplayName,
                EmailAddress = attendee.EmailAddress,
                Fullname = attendee.Fullname,
                Pronoun = attendee.Pronoun,
                Slug = attendee.UserId.ToString(),
                IsHost = attendee.Host,
                IsSpeaker = attendee.Speaker,
                IsAttending = attendee.SignedUp,
                NumberOfGuests = attendee.NumberOfGuests,
                ProfilePicture = _urlBuilder.BuildUserDetailsProfilePictureURL(attendee.UserId),
                DietaryRequirements = SensitiveField(canViewDetails, attendee.DietaryRequirements),
                Answers = SensitiveArray(canViewDetails, () => attendee.Answers.Select(q => MapQuestion(q, theEvent.Questions))),
            }).ToArray();
        }

        private static T[] SensitiveArray<T>(bool canViewDetails, Func<IEnumerable<T>> fn)
        {
            if (canViewDetails)
            {
                return fn().ToArray();
            }
            else
            {
                return Array.Empty<T>();
            }
        }

        private static string? SensitiveField(bool canViewDetails, string fieldValue)
        {
            if (canViewDetails)
            {
                return fieldValue;
            }
            else
            {
                return null;
            }
        }

        private static ViewModels.Answer MapQuestion(Models.Answer answer, System.Collections.Generic.List<Models.Question> questions)
        {
            var question = questions.SingleOrDefault(q => q.Code == answer.Code);

            return new Answer
            {
                Caption = answer.Caption,
                Code = answer.Code,
                QuestionType = question?.QuestionType.ToString(),
                UsersAnswer = answer.UsersAnswer,
            };
        }
    }
}