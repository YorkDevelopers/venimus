using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class GroupEvent
    {
        /// <summary>
        ///     The unique internal ID for the event
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        ///     The unique external id for the event.  For example May2019
        /// </summary>
        public string Slug { get; set; } = default!;

        /// <summary>
        ///     The internal ID of the group to which this event belongs
        /// </summary>
        public ObjectId GroupId { get; set; }

        /// <summary>
        ///     The external ID of the group to which this event belongs
        /// </summary>
        public string GroupSlug { get; set; } = default!;

        /// <summary>
        ///     The name of the group the event belongs to
        /// </summary>
        public string GroupName { get; set; } = default!;

        /// <summary>
        /// The title of the event,  for example March 2019 Meetup. Must be unique for the group.
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// A description of the event,  in markdown format.
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// The location of the event, for example York Minster,  Room 201 York St John University
        /// </summary>
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
        /// Attendees of this group
        /// </summary>
        public List<GroupEventAttendees> Members { get; set; } = new List<GroupEventAttendees>();

        /// <summary>
        ///     Questions to ask the user when they are registering
        /// </summary>
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
