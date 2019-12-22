using System;

namespace YorkDeveloperEvents.ViewModels
{
    public class GetEvent
    {
        /// <summary>
        ///     The unique external ID for the event.
        /// </summary>
        public string EventSlug { get; set; }

        /// <summary>
        ///     The name of the group hosting the event.  For example YorkCodeDojo
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        ///     The title of the event.  Monthly meeting - October
        /// </summary>
        public string EventTitle { get; set; }

        /// <summary>
        ///     A description of the event in markdown.
        /// </summary>
        public string EventDescription { get; set; }

        /// <summary>
        ///     When does the event start,  in UTC time?
        /// </summary>
        public DateTime EventStartsUTC { get; set; }

        /// <summary>
        ///     When does the event finish, in UTC time?
        /// </summary>
        public DateTime EventFinishesUTC { get; set; }

        /// <summary>
        ///     Where is the event being held?
        /// </summary>
        public string EventLocation { get; set; }

        /// <summary>
        ///     How many people + guests are allowed to sign up.  This includes hosts and speakers
        /// </summary>
        public int MaximumNumberOfAttendees { get; set; }

        /// <summary>
        ///     Questions to ask the user when they are registering
        /// </summary>
        public Question[] Questions { get; set; } = Array.Empty<Question>();

        /// <summary>
        ///     Are members allowed to bring guests to this event?
        /// </summary>
        public bool GuestsAllowed { get; set; }
    }
}
