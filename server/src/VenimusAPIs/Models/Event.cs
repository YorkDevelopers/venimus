using System;
using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Required for Mongo")]
        public ObjectId _id { get; set; }

        /// <summary>
        /// Unique ID for the event
        /// </summary>
        public string EventID { get; set; }

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
    }
}
