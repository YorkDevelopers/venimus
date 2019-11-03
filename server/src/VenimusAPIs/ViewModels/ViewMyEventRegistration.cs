namespace VenimusAPIs.ViewModels
{
    public class ViewMyEventRegistration
    {
        /// <summary>
        ///     The number of unregistered guests joining the user
        /// </summary>
        public int NumberOfGuests { get; set; }

        /// <summary>
        ///     Any Dietary Requirements.
        /// </summary>
        public string DietaryRequirements { get; set; }

        /// <summary>
        ///     Free format message to the event host.
        /// </summary>
        public string MessageToOrganiser { get; set; }

        /// <summary>
        ///     Has the user said they are attending the event?
        /// </summary>
        public bool Attending { get; set; }
    }
}
