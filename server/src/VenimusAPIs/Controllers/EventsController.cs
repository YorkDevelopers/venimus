﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Extensions;
using VenimusAPIs.Mongo;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class EventsController : BaseController
    {
        private readonly UserStore _userStore;
        private readonly EventStore _eventStore;

        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;
        private readonly GetFutureEventsQuery _getFutureEventsQuery;

        public EventsController(Mongo.UserStore userStore, Mongo.EventStore eventStore, IStringLocalizer<ResourceMessages> stringLocalizer, GetFutureEventsQuery getFutureEventsQuery)
        {
            _userStore = userStore;
            _eventStore = eventStore;
            _stringLocalizer = stringLocalizer;
            _getFutureEventsQuery = getFutureEventsQuery;
        }

        private static ViewModels.Question MapQuestion(Models.Question model)
        {
            return new Question
            {
                Caption = model.Caption,
                Code = model.Code,
                QuestionType = model.QuestionType.ToString(),
            };
        }

        private static Models.Question MapQuestion(ViewModels.Question viewModel)
        {
            return new Models.Question
            {
                Caption = viewModel.Caption,
                Code = viewModel.Code,
                QuestionType = (Models.QuestionType)Enum.Parse(typeof(Models.QuestionType), viewModel.QuestionType, true),
            };
        }

        /// <summary>
        ///     Allows you to create a new event for your group.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/groups/YorkCodeDojo/events
        ///     {
        ///         "slug" : "Oct2019"
        ///         "title" : "Game of Life - Oct 2019",
        ///         "description" : "Tonight we will work in pairs implementing the **classic Game Of Life**"
        ///         "location" : "Room 12"
        ///         "startTime" : "2019-12-12 18:30"
        ///         "endTime" : "2019-12-12 21:00"
        ///     }
        ///
        /// </remarks>
        /// <returns>The route to the created event</returns>
        /// <response code="201">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">The group does not exist.</response>
        [Route("api/groups/{groupSlug}/events")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(Models.Group? group, [FromBody] CreateEvent newEvent)
        {
            if (group is null)
            {
                return NotFound();
            }

            var caller = await _userStore.GetUserByID(UniqueIDForCurrentUser).ConfigureAwait(false);
            if (caller == null)
            {
                return Forbid();
            }

            if (!group.UserIsGroupAdministrator(caller))
            {
                return Forbid();
            }

            if (newEvent.StartTimeUTC < DateTime.UtcNow)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_IN_THE_PAST).Value;
                ModelState.AddModelError(nameof(newEvent.StartTimeUTC), message);
            }

            if (newEvent.StartTimeUTC >= newEvent.EndTimeUTC)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_END_BEFORE_START).Value;
                ModelState.AddModelError(nameof(newEvent.EndTimeUTC), message);
            }

            var duplicate = await _eventStore.GetEvent(group.Slug, newEvent.Slug).ConfigureAwait(false);
            if (duplicate != null)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_ALREADY_EXISTS).Value;
                ModelState.AddModelError(nameof(newEvent.Slug), message);
            }

            var duplicateQuestion = newEvent.Questions.GroupBy(q => q.Code).Any(grp => grp.Count() > 1);
            if (duplicateQuestion)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_DUPLICATE_QUESTION).Value;
                ModelState.AddModelError(nameof(Question.Code), message);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var model = new Models.GroupEvent
            {
                GroupSlug = group.Slug,
                GroupId = group!.Id,
                GroupName = group.Name,
                Description = newEvent.Description,
                EndTimeUTC = newEvent.EndTimeUTC,
                FoodProvided = newEvent.FoodProvided,
                GuestsAllowed = newEvent.GuestsAllowed,
                Location = newEvent.Location,
                MaximumNumberOfAttendees = newEvent.MaximumNumberOfAttendees,
                Slug = newEvent.Slug,
                StartTimeUTC = newEvent.StartTimeUTC,
                Title = newEvent.Title,
                Questions = newEvent.Questions.Select(q => MapQuestion(q)).ToList(),
            };

            await _eventStore.StoreEvent(model).ConfigureAwait(false);
            return CreatedAtRoute("Events", new { groupSlug = group.Slug, eventSlug = model.Slug }, newEvent);
        }

        /// <summary>
        ///     Allows you to amend the details of an existing event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/groups/YorkCodeDojo/events/12345
        ///     {
        ///         "slug" : "Oct2019"
        ///         "title" : "Game of Life - Oct 2019",
        ///         "description" : "Tonight we will work in pairs implementing the **classic Game Of Life**"
        ///         "location" : "Room 12"
        ///         "startTime" : "2019-12-12 18:30"
        ///         "endTime" : "2019-12-12 21:00"
        ///         "host" : "E Betteridge"
        ///         "speaker" : "J Betteridge"
        ///     }
        ///
        /// </remarks>
        /// <returns>No data</returns>
        /// <response code="204">Success</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="404">The group or event does not exist.</response>
        [Route("api/groups/{groupSlug}/events/{eventSlug}")]
        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(Models.Group? group, [FromRoute, Slug] string eventSlug, [FromBody] UpdateEvent amendedEvent)
        {
            if (group is null) return NotFound();

            var caller = await _userStore.GetUserByID(UniqueIDForCurrentUser).ConfigureAwait(false);
            if (caller == null)
            {
                return Forbid();
            }

            if (!group.UserIsGroupAdministrator(caller))
            {
                return Forbid();
            }

            var model = await _eventStore.GetEvent(group.Slug, eventSlug).ConfigureAwait(false);
            if (model == null)
            {
                return NotFound();
            }

            if (!model.Slug.Equals(amendedEvent.Slug, StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicate = await _eventStore.GetEvent(group.Slug, amendedEvent.Slug).ConfigureAwait(false);
                if (duplicate != null)
                {
                    var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_ALREADY_EXISTS).Value;
                    ModelState.AddModelError(nameof(amendedEvent.Slug), message);
                }
            }

            if (amendedEvent.StartTimeUTC >= amendedEvent.EndTimeUTC)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_END_BEFORE_START).Value;
                ModelState.AddModelError(nameof(amendedEvent.EndTimeUTC), message);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            model.Description = amendedEvent.Description;
            model.EndTimeUTC = amendedEvent.EndTimeUTC;
            model.FoodProvided = amendedEvent.FoodProvided;
            model.GuestsAllowed = amendedEvent.GuestsAllowed;
            model.Location = amendedEvent.Location;
            model.MaximumNumberOfAttendees = amendedEvent.MaximumNumberOfAttendees;
            model.Slug = amendedEvent.Slug;
            model.StartTimeUTC = amendedEvent.StartTimeUTC;
            model.Title = amendedEvent.Title;
            model.Questions = amendedEvent.Questions.Select(q => MapQuestion(q)).ToList();

            await _eventStore.UpdateEvent(model).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        ///     Allows an administrator to delete an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/groups/YorkCodeDojo/events/1234
        ///
        /// </remarks>
        /// <returns>NoContent</returns>
        /// <response code="204">Success</response>
        [Authorize]
        [Route("api/groups/{groupSlug}/events/{eventSlug}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Delete(Models.Group? group, [FromRoute] string eventSlug)
        {
            if (group is null) return NotFound();

            var caller = await _userStore.GetUserByID(UniqueIDForCurrentUser).ConfigureAwait(false);
            if (caller == null)
            {
                return Forbid();
            }

            if (!UserIsASystemAdministrator && !group.UserIsGroupAdministrator(caller))
            {
                return Forbid();
            }

            var model = await _eventStore.GetEvent(group.Slug, eventSlug).ConfigureAwait(false);
            if (model != null)
            {
                if (model.EndTimeUTC < DateTime.UtcNow)
                {
                    var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_HAS_HAPPENED).Value;
                    var details = new ValidationProblemDetails { Detail = message };
                    return ValidationProblem(details);
                }

                await _eventStore.DeleteEvent(model).ConfigureAwait(false);
            }

            return NoContent();
        }

        /// <summary>
        ///     Allows you to retrieve the details of an event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo/events/1234
        ///
        /// </remarks>
        /// <returns>The GetEvent view model</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Group or Event does not exist.</response>
        [Authorize]
        [Route("api/groups/{groupSlug}/events/{eventSlug}", Name = "Events")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetEvent>> Get(Models.Group? group, [FromRoute, Slug] string eventSlug)
        {
            if (group is null) return NotFound();

            var caller = await _userStore.GetUserByID(UniqueIDForCurrentUser).ConfigureAwait(false);
            if (caller == null)
            {
                return Forbid();
            }

            if (!UserIsASystemAdministrator && !group.UserIsGroupMember(caller))
            {
                return Forbid();
            }

            var model = await _eventStore.GetEvent(group.Slug, eventSlug).ConfigureAwait(false);
            if (model == null)
            {
                return NotFound();
            }

            return new GetEvent
            {
                EventDescription = model.Description,
                EventFinishesUTC = model.EndTimeUTC,
                EventSlug = eventSlug,
                EventStartsUTC = model.StartTimeUTC,
                EventTitle = model.Title,
                MaximumNumberOfAttendees = model.MaximumNumberOfAttendees,
                GuestsAllowed = model.GuestsAllowed,
                GroupName = model.GroupName,
                EventLocation = model.Location,
                Questions = model.Questions.Select(q => MapQuestion(q)).ToArray(),
            };
        }

        /// <summary>
        ///     Allows you to retrieve the list of all events for this group
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/groups/YorkCodeDojo/events
        ///
        /// </remarks>
        /// <returns>The array of ListEventsForGroup view models</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Group does not exist.</response>
        [Route("api/groups/{groupSlug}/events")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ViewModels.ListEventsForGroup[]>> Get([FromRoute, Slug] string groupSlug, [FromQuery] bool includePastEvents = false, [FromQuery] bool includeFutureEvents = true)
        {
            return await _eventStore.GetEvents(groupSlug, includePastEvents, includeFutureEvents).ConfigureAwait(false);
        }

        /// <summary>
        ///     Allows you request the list of events.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Events?eventsIHaveSignedUpToOnly=false
        ///
        /// </remarks>
        /// <returns>Array of ListEvent view models</returns>
        /// <response code="200">Success</response>
        [Route("api/Events")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListEvents>>> Get([FromQuery]bool eventsIHaveSignedUpToOnly = false)
        {
            if (eventsIHaveSignedUpToOnly)
            {
                var uniqueID = UniqueIDForCurrentUser;

                var caller = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);

                return await _eventStore.GetMyEventRegistrations(caller.Id).ConfigureAwait(false);
            }
            else
            {
                return await _getFutureEventsQuery.Evaluate().ConfigureAwait(false);
            }
        }
    }
}