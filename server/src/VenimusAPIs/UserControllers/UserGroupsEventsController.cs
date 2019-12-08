using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserGroupsEventsController : BaseUserController
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.UserStore _userStore;
        private readonly IStringLocalizer<Messages> _stringLocalizer;

        public UserGroupsEventsController(Mongo.EventStore eventStore, Mongo.UserStore userStore, IStringLocalizer<Messages> stringLocalizer)
        {
            _eventStore = eventStore;
            _userStore = userStore;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        ///     Allows the current user to sign up to an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/user/groups/YorkCodeDojo/Events
        ///     {
        ///         "eventSlug" : "Nov2019",
        ///         "numberOfGuest" : 1,
        ///         "dietaryRequirements" : "Milk free",
        ///         "messageToOrganiser" : "I might be 10 minutes late"
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the created group</returns>
        /// <response code="204">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">Group does not exist</response>
        [Authorize]
        [CallerMustBeGroupMember]
        [HttpPost]
        [Route("api/User/Groups/{groupSlug}/Events")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromRoute, Slug] string groupSlug, [FromBody] RegisterForEvent signUpDetails)
        {
            var eventSlug = signUpDetails.EventSlug;
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug);
            if (theEvent == null)
            {
                return NotFound();
            }

            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _userStore.GetUserByID(uniqueID);

            var member = theEvent.Members.SingleOrDefault(m => m.UserId == existingUser.Id);
            if (member != null)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.USER_ALREADY_SIGNED_UP).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            if (signUpDetails.NumberOfGuests > 0 && !theEvent.GuestsAllowed)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.GUESTS_NOT_ALLOWED).Value;
                ModelState.AddModelError(nameof(RegisterForEvent.NumberOfGuests), message);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var numberAttending = theEvent.Members.Where(member => member.SignedUp).Sum(member => member.NumberOfGuests + 1);
            if ((numberAttending + 1 + signUpDetails.NumberOfGuests) > theEvent.MaximumNumberOfAttendees)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.EVENT_IS_FULL).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            if (theEvent.EndTimeUTC < DateTime.UtcNow)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.EVENT_HAS_TAKEN_PLACE).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            theEvent.Members.Add(new Models.Event.EventAttendees
            {
                DietaryRequirements = signUpDetails.DietaryRequirements,
                MessageToOrganiser = signUpDetails.MessageToOrganiser,
                NumberOfGuests = signUpDetails.NumberOfGuests,
                UserId = existingUser.Id,
                SignedUp = true,
                Bio = existingUser.Bio,
                DisplayName = existingUser.DisplayName,
                EmailAddress = existingUser.EmailAddress,
                Fullname = existingUser.Fullname,
                Pronoun = existingUser.Pronoun,
            });

            await _eventStore.UpdateEvent(theEvent);

            return CreatedAtRoute("EventRegistration", new { groupSlug, eventSlug }, null);
        }

        /// <summary>
        ///     Allows the current user to update their registration for an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/user/groups/YorkCodeDojo/Events/Nov2019
        ///     {
        ///         "numberOfGuest" : 1,
        ///         "dietaryRequirements" : "Milk free",
        ///         "messageToOrganiser" : "I might be 10 minutes late"
        ///     }
        ///
        /// </remarks>
        /// <returns>NoContent</returns>
        /// <response code="204">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">Group does not exist</response>
        [Authorize]
        [HttpPut]
        [CallerMustBeGroupMember]
        [Route("api/User/Groups/{groupSlug}/Events/{eventSlug}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromRoute, Slug] string groupSlug, [FromRoute, Slug] string eventSlug, [FromBody] AmendRegistrationForEvent newDetails)
        {
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug);
            if (theEvent == null)
            {
                return NotFound();
            }

            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _userStore.GetUserByID(uniqueID);

            var created = false;
            var member = theEvent.Members.SingleOrDefault(m => m.UserId == existingUser.Id);
            if (member == null)
            {
                member = new Models.Event.EventAttendees
                {
                    UserId = existingUser.Id,
                };

                theEvent.Members.Add(member);
                created = true;
            }

            if (newDetails.NumberOfGuests > 0 && !theEvent.GuestsAllowed)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.GUESTS_NOT_ALLOWED).Value;
                ModelState.AddModelError(nameof(AmendRegistrationForEvent.NumberOfGuests), message);
            }

            var numberAttending = theEvent.Members.Where(member => member.SignedUp).Sum(member => member.NumberOfGuests + 1);
            var delta = (newDetails.NumberOfGuests - member.NumberOfGuests) + (member.SignedUp ? 0 : 1);

            if ((numberAttending + delta) > theEvent.MaximumNumberOfAttendees)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.TOO_MANY_PEOPLE).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (theEvent.EndTimeUTC < DateTime.UtcNow)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.EVENT_HAS_TAKEN_PLACE).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            member.SignedUp = true;
            member.DietaryRequirements = newDetails.DietaryRequirements;
            member.NumberOfGuests = newDetails.NumberOfGuests;
            member.MessageToOrganiser = newDetails.MessageToOrganiser;

            await _eventStore.UpdateEvent(theEvent);

            if (created)
            {
                return CreatedAtRoute("EventRegistration", new { groupSlug, eventSlug }, null);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        ///     Allows the current user to decline attending an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/user/groups/YorkCodeDojo/Events/Nov2019
        ///
        /// </remarks>
        /// <returns>NoContent</returns>
        /// <response code="204">Success</response>
        /// <response code="400">Event has already happened</response>
        /// <response code="404">Group or Event does not exist.</response>
        [Authorize]
        [Route("api/user/groups/{groupSlug}/events/{eventSlug}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute, Slug] string groupSlug, [FromRoute, Slug] string eventSlug)
        {
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug);
            if (theEvent == null)
            {
                return NotFound();
            }

            if (theEvent.EndTimeUTC < DateTime.UtcNow)
            {
                var message = _stringLocalizer.GetString(Resources.Messages.EVENT_HAS_TAKEN_PLACE).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            var member = await GetUsersRegistrationForThisEvent(theEvent);

            member.SignedUp = false;

            await _eventStore.UpdateEvent(theEvent);

            return NoContent();
        }

        private async Task<Models.Event.EventAttendees> GetUsersRegistrationForThisEvent(Models.Event theEvent)
        {
            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _userStore.GetUserByID(uniqueID);

            var member = theEvent.Members.SingleOrDefault(m => m.UserId == existingUser.Id);
            if (member == null)
            {
                member = new Models.Event.EventAttendees
                {
                    UserId = existingUser.Id,
                };

                theEvent.Members.Add(member);
            }

            return member;
        }

        /// <summary>
        ///     Allows the current user to retrieve their registration details for an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/user/groups/YorkCodeDojo/Events/Nov2019
        ///
        /// </remarks>
        /// <returns>The GetEvent view model</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Group or Event does not exist.</response>
        [Authorize]
        [Route("api/user/groups/{groupSlug}/events/{eventSlug}", Name = "EventRegistration")]
        [CallerMustBeGroupMember(CanBeSystemAdministratorInstead = false)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ViewMyEventRegistration>> Get([FromRoute] string groupSlug, [FromRoute] string eventSlug)
        {
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug);
            if (theEvent == null)
            {
                return NotFound();
            }

            var uniqueID = UniqueIDForCurrentUser;
            var existingUser = await _userStore.GetUserByID(uniqueID);

            var member = theEvent.Members.SingleOrDefault(m => m.UserId == existingUser.Id);
            if (member == null)
            {
                return NotFound();
            }

            return new ViewMyEventRegistration
            {
                DietaryRequirements = member.DietaryRequirements,
                NumberOfGuests = member.NumberOfGuests,
                MessageToOrganiser = member.MessageToOrganiser,
                Attending = member.SignedUp,
            };
        }
    }
}