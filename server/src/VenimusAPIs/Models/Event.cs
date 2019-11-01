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
        /// The group to which this event belongs
        /// </summary>
        public ObjectId GroupId { get; set; }

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
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Date and time the event ends.  Must be after the StartTime
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Attendees of this group
        /// </summary>
        public List<EventAttendees>[] Members { get; set; }

        public class EventAttendees
        {
            public ObjectId UserId { get; set; }
            
            public bool SignedUp { get; set; }
            
            public bool Attended { get; set; }

            public bool Host { get; set; }
            
            public bool Speaker { get; set; }
        }
    }
}
