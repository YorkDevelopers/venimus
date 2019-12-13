using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.UserControllers;
using VenimusAPIs.Validation;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Controllers
{
    [ApiController]
    public class EventsController : BaseUserController
    {
        private readonly Mongo.EventStore _eventStore;
        private readonly Mongo.GroupStore _groupStore;

        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ResourceMessages> _stringLocalizer;

        public EventsController(Mongo.EventStore eventStore, Mongo.GroupStore groupStore, IMapper mapper, IStringLocalizer<ResourceMessages> stringLocalizer)
        {
            _eventStore = eventStore;
            _groupStore = groupStore;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
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
        [CallerMustBeGroupAdministrator]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromRoute, Slug] string groupSlug, [FromBody] CreateEvent newEvent)
        {
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

            var duplicate = await _eventStore.GetEvent(groupSlug, newEvent.Slug).ConfigureAwait(false);
            if (duplicate != null)
            {
                var message = _stringLocalizer.GetString(Resources.ResourceMessages.EVENT_ALREADY_EXISTS).Value;
                ModelState.AddModelError(nameof(newEvent.Slug), message);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);

            var model = _mapper.Map<Models.GroupEvent>(newEvent);
            model.GroupSlug = groupSlug;
            model.GroupId = group!.Id;
            model.GroupName = group.Name;

            await _eventStore.StoreEvent(model).ConfigureAwait(false);
            return CreatedAtRoute("Events", new { groupSlug = groupSlug, eventSlug = model.Slug }, newEvent);
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
        [CallerMustBeGroupAdministrator]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put([FromRoute, Slug] string groupSlug, [FromRoute, Slug] string eventSlug, [FromBody] UpdateEvent amendedEvent)
        {
            var model = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
            if (model == null)
            {
                return NotFound();
            }

            if (!model.Slug.Equals(amendedEvent.Slug, StringComparison.InvariantCultureIgnoreCase))
            {
                var duplicate = await _eventStore.GetEvent(groupSlug, amendedEvent.Slug).ConfigureAwait(false);
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

            _mapper.Map(amendedEvent, model);

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
        [CallerMustBeGroupAdministrator]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromRoute] string groupSlug, [FromRoute] string eventSlug)
        {
            var model = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
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
        [CallerMustBeGroupMember]
        [Route("api/groups/{groupSlug}/events/{eventSlug}", Name = "Events")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetEvent>> Get([FromRoute, Slug] string groupSlug, [FromRoute, Slug] string eventSlug)
        {
            var model = await _eventStore.GetEvent(groupSlug, eventSlug).ConfigureAwait(false);
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
                Questions = AddNumberOfGuestsQuestionIfApplicable(model.GuestsAllowed, model.Questions.Select(q => MapQuestion(q)).ToArray()),
            };
        }

        private ViewModels.Question CreateNumberOfGuestsQuestion()
        {
            return new Question
            {
                Caption = _stringLocalizer.GetString(Resources.ResourceMessages.QUESTION_NUMBEROFGUESTS).Value,
                Code = "NumberOfGuests",
                QuestionType = Models.QuestionType.NumberOfGuests.ToString(),
            };
        }

        private Question[] AddNumberOfGuestsQuestionIfApplicable(bool guestsAllowed, Question[] questions)
        {
            if (guestsAllowed)
            {
                var extraQuestion = new Question[] { CreateNumberOfGuestsQuestion() };
                questions = extraQuestion.Union(questions).ToArray();
            }

            return questions;
        }
    }
}