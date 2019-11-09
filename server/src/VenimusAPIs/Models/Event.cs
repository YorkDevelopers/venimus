using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class Event
    {
        /// <summary>
        ///     The unique internal ID for the event
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        ///     The unique external id for the event.  For example May2019
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        ///     The internal ID of the group to which this event belongs
        /// </summary>
        public ObjectId GroupId { get; set; }

        /// <summary>
        ///     The external ID of the group to which this event belongs
        /// </summary>
        public string GroupSlug { get; set; }

        /// <summary>
        ///     The name of the group the event belongs to
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The title of the event,  for example March 2019 Meetup. Must be unique for the group.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A description of the event,  in markdown format.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The location of the event, for example York Minster,  Room 201 York St John University
        /// </summary>
        public string Location { get; set; }

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
        public List<EventAttendees> Members { get; set; }

        public class EventAttendees
        {
            public ObjectId UserId { get; set; }

            /// <summary>
            ///     Are they still signed up
            /// </summary>
            public bool SignedUp { get; set; }

            /// <summary>
            ///     Did they attend the event?
            /// </summary>
            public bool? Attended { get; set; }

            /// <summary>
            ///     Are they organising the event
            /// </summary>
            public bool Host { get; set; }

            /// <summary>
            ///     Are they presenting at the event
            /// </summary>
            public bool Speaker { get; set; }

            /// <summary>
            ///     Any Dietary Requirements the user has
            /// </summary>
            public string DietaryRequirements { get; set; }

            /// <summary>
            ///     Free format message to the organiser
            /// </summary>
            public string MessageToOrganiser { get; set; }

            /// <summary>
            ///     The number of unregistered guest the person is bringing
            /// </summary>
            public int NumberOfGuests { get; set; }

            /// <summary>
            ///     Are members allowed to bring guests to this event?
            /// </summary>
            public bool GuestsAllowed { get; set; }
        }
    }
}
