using System;

namespace VenimusAPIs.ViewModels
{
    public class ListEventsForGroup
    {
        /// <summary>
        ///     The unique external ID for the event.
        /// </summary>
        public string EventSlug { get; set; } = default!;

        /// <summary>
        ///     The title of the event.  Monthly meeting - October
        /// </summary>
        public string EventTitle { get; set; } = default!;

        /// <summary>
        ///     A description of the event in markdown.
        /// </summary>
        public string EventDescription { get; set; } = default!;

        /// <summary>
        ///     When does the event start,  in UTC time?
        /// </summary>
        public DateTime EventStartsUTC { get; set; }

        /// <summary>
        ///     When does the event finish, in UTC time?
        /// </summary>
        public DateTime EventFinishesUTC { get; set; }
    }
}
