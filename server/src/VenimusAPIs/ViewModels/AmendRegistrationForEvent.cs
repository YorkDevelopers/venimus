using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.ViewModels
{
    public class AmendRegistrationForEvent
    {
        /// <summary>
        ///     The number of unregistered guests joining the user
        /// </summary>
        [Range(0, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        /// <summary>
        ///     Any Dietary Requirements.
        /// </summary>
        public string DietaryRequirements { get; set; } = default!;

        /// <summary>
        ///     Free format message to the event host.
        /// </summary>
        public string MessageToOrganiser { get; set; } = default!;
    }
}
