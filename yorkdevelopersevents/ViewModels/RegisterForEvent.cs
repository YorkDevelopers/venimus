using System;
using System.ComponentModel.DataAnnotations;

namespace YorkDeveloperEvents.ViewModels
{
    public class RegisterForEvent
    {
        /// <summary>
        ///     The unique ID for the event.
        /// </summary>
        public string EventSlug { get; set; }

        /// <summary>
        ///     The number of unregistered guests joining the user
        /// </summary>
        [Range(0, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        /// <summary>
        ///     Any Dietary Requirements.
        /// </summary>
        public string DietaryRequirements { get; set; }

        /// <summary>
        ///     Free format message to the event host.
        /// </summary>
        public string MessageToOrganiser { get; set; }
    }
}
