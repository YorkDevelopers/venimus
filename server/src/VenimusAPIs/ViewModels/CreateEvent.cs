using System;
using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.ViewModels
{
    public class CreateEvent
    {
        /// <summary>
        ///     The unique external id for the event.  For example May2019
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; }

        /// <summary>
        /// The title of the event,  for example March 2019 Meetup. Must be unique for the group.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// A description of the event,  in markdown format.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// The location of the event, for example York Minster,  Room 201 York St John University
        /// </summary>
        [Required]
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
