using System;

namespace VenimusAPIs.ViewModels
{
    public class ViewMyEventRegistration
    {
        /// <summary>
        ///     Has the user said they are attending the event?
        /// </summary>
        public bool Attending { get; set; }

        /// <summary>
        ///     The group hosting the event
        /// </summary>
        public string GroupName { get; set; } = default!;

        /// <summary>
        ///     The title of the event
        /// </summary>
        public string EventTitle { get; set; } = default!;

        /// <summary>
        ///     Any information we need to gather from the user
        /// </summary>
        public Answer[] Answers { get; set; } = Array.Empty<Answer>();
    }
}
