using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YorkDeveloperEvents.ViewModels
{
    public class UpdateEvent
    {
        /// <summary>
        ///     The unique external id for the event.  For example May2019
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = default!;

        /// <summary>
        /// The title of the event,  for example March 2019 Meetup. Must be unique for the group.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// A description of the event,  in markdown format.
        /// </summary>
        [Required]
        public string Description { get; set; } = default!;

        /// <summary>
        /// The location of the event, for example York Minster,  Room 201 York St John University
        /// </summary>
        [Required]
        public string Location { get; set; } = default!;

        /// <summary>
        /// Date and time the event starts
        /// </summary>
        public DateTime StartTimeUTC { get; set; }

        /// <summary>
        /// Date and time the event ends.  Must be after the StartTime
        /// </summary>
        public DateTime EndTimeUTC { get; set; }

        /// <summary>
        ///     How many people + guests are allowed to sign up.  This includes hosts and speakers
        /// </summary>
        public int MaximumNumberOfAttendees { get; set; }

        /// <summary>
        ///     Are members allowed to bring guests to this event?
        /// </summary>
        public bool GuestsAllowed { get; set; }

        /// <summary>
        ///     Any additional information we need to obtain from the user.
        /// </summary>
        public List<Question> Questions { get; set; } = new List<Question>();

        /// <summary>
        ///     Will food be provided at this event?
        /// </summary>
        public bool FoodProvided { get; set; }
    }
}
