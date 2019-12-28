using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.UserControllers
{
    [ApiController]
    public class UserGroupsEventsController : BaseUserController
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.UserStore _userStore;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;

        public UserGroupsEventsController(Mongo.EventStore eventStore, Mongo.UserStore userStore, IStringLocalizer<ResourceMessages> stringLocalizer)
        {
            _eventStore = eventStore;
            _userStore = userStore;
            _stringLocalizer = stringLocalizer;
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
        public async Task<IActionResult> Put([FromRoute, Slug] string groupSlug, [FromRoute, Slug] string eventSlug, [FromBody] RegisterForEvent newDetails)
        {
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
            if (theEvent == null)
            {
                return NotFound();
            }

            var numberOfGuestsAnswer = newDetails.Answers.SingleOrDefault(a => a.Code == "NumberOfGuests");
            if (!int.TryParse(numberOfGuestsAnswer?.UsersAnswer ?? "0", out var numberOfGuests) || numberOfGuests < 0)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.NUMBER_OF_GUESTS_IS_NEGATIVE).Value;
                ModelState.AddModelError("NumberOfGuests", message);
                return ValidationProblem(ModelState);
            }

            var dietaryRequirementsAnswer = newDetails.Answers.SingleOrDefault(a => a.Code == "DietaryRequirements");
            var dietaryRequirements = dietaryRequirementsAnswer?.UsersAnswer ?? string.Empty;

            newDetails.Answers = newDetails.Answers.Where(a => a.Code != "NumberOfGuests" && a.Code != "DietaryRequirements").ToList();

            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            var created = false;
            var member = theEvent.Members.SingleOrDefault(m => m.UserId == existingUser.Id);
            if (member == null)
            {
                member = new Models.GroupEventAttendee
                {
                    UserId = existingUser.Id,
                    Bio = existingUser.Bio,
                    DisplayName = existingUser.DisplayName,
                    EmailAddress = existingUser.EmailAddress,
                    Fullname = existingUser.Fullname,
                    Pronoun = existingUser.Pronoun,
                };

                theEvent.Members.Add(member);
                created = true;
            }

            if (numberOfGuests > 0 && !theEvent.GuestsAllowed)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.GUESTS_NOT_ALLOWED).Value;
                ModelState.AddModelError("NumberOfGuests", message);
            }

            var numberAttending = theEvent.Members.Where(member => member.SignedUp).Sum(member => member.NumberOfGuests + 1);
            var delta = (numberOfGuests - member.NumberOfGuests) + (member.SignedUp ? 0 : 1);

            if ((numberAttending + delta) > theEvent.MaximumNumberOfAttendees)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.TOO_MANY_PEOPLE).Value;
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
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_HAS_TAKEN_PLACE).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            member.SignedUp = true;
            member.DietaryRequirements = dietaryRequirements;
            member.NumberOfGuests = numberOfGuests;
            member.Answers = newDetails.Answers.Select(q => MapAnswer(q, theEvent.Questions)).ToList();

            await _eventStore.UpdateEvent(theEvent).ConfigureAwait(false);

            if (created)
            {
                return CreatedAtRoute("EventRegistration", new { groupSlug, eventSlug }, null);
            }
            else
            {
                return NoContent();
            }
        }

        private static Models.Answer MapAnswer(ViewModels.Answer answer, List<Models.Question> questions)
        {
            var question = questions.SingleOrDefault(q => q.Code == answer.Code);
            if (question != null)
            {
                answer.Caption = question.Caption;
            }

            return new Models.Answer
            {
                Caption = answer.Caption ?? string.Empty,
                Code = answer.Code,
                UsersAnswer = answer.UsersAnswer,
            };
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
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
            if (theEvent == null)
            {
                return NotFound();
            }

            if (theEvent.EndTimeUTC < DateTime.UtcNow)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_HAS_TAKEN_PLACE).Value;
                return ValidationProblem(new ValidationProblemDetails
                {
                    Detail = message,
                });
            }

            var member = await GetUsersRegistrationForThisEvent(theEvent).ConfigureAwait(false);

            member.SignedUp = false;

            await _eventStore.UpdateEvent(theEvent).ConfigureAwait(false);

            return NoContent();
        }

        private async Task<Models.GroupEventAttendee> GetUsersRegistrationForThisEvent(Models.GroupEvent theEvent)
        {
            var uniqueID = UniqueIDForCurrentUser;

            var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            var member = theEvent.Members.SingleOrDefault(m => m.UserId == existingUser.Id);
            if (member == null)
            {
                member = new Models.GroupEventAttendee
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
            var theEvent = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
            if (theEvent == null)
            {
                return NotFound();
            }

            var allQuestions = theEvent.Questions;

            if (theEvent.GuestsAllowed)
            {
                allQuestions.Add(CreateNumberOfGuestsQuestion());
            }

            if (theEvent.FoodProvided)
            {
                allQuestions.Add(CreateDietaryRequirementsQuestion());
            }

            var uniqueID = UniqueIDForCurrentUser;
            var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

            var member = theEvent.Members.SingleOrDefault(m => m.UserId == existingUser.Id);

            var result = new ViewMyEventRegistration
            {
                Attending = false,
                EventTitle = theEvent.Title,
                GroupName = theEvent.GroupName,
                Answers = allQuestions.Select(q => MapQuestionToAnswer(q, member)).ToArray(),
            };

            if (member != null)
            {
                result.Attending = member.SignedUp;
            }

            return result;
        }

        private static Answer MapQuestionToAnswer(Models.Question question, Models.GroupEventAttendee? member)
        {
            var usersAnswer = question.Code switch
            {
                _ when member == null => string.Empty,
                "NumberOfGuests" => member.NumberOfGuests.ToString(CultureInfo.InvariantCulture),
                "DietaryRequirements" => member.DietaryRequirements,
                _ => member.Answers.SingleOrDefault(a => a.Code == question.Code)?.UsersAnswer ?? string.Empty
            };

            return new Answer
            {
                Caption = question.Caption,
                Code = question.Code,
                QuestionType = question.QuestionType.ToString(),
                UsersAnswer = usersAnswer,
            };
        }

        private Models.Question CreateNumberOfGuestsQuestion()
        {
            return new Models.Question
            {
                Caption = _stringLocalizer.GetString(Resources.ResourceMessages.QUESTION_NUMBEROFGUESTS).Value,
                Code = "NumberOfGuests",
                QuestionType = Models.QuestionType.NumberOfGuests,
            };
        }

        private Models.Question CreateDietaryRequirementsQuestion()
        {
            return new Models.Question
            {
                Caption = _stringLocalizer.GetString(Resources.ResourceMessages.QUESTION_DIETARYREQUIREMENTS).Value,
                Code = "DietaryRequirements",
                QuestionType = Models.QuestionType.DietaryRequirements,
            };
        }
    }
}