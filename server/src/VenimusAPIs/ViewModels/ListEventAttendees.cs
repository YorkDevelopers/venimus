using System;

namespace VenimusAPIs.ViewModels
{
    public class ListEventAttendees
    {
        /// <summary>
        ///     The external ID for the user.
        /// </summary>
        public string Slug { get; set; } = default!;

        /// <summary>
        ///     The email address which also links all the social media accounts together.
        /// </summary>
        public string EmailAddress { get; set; } = default!;

        /// <summary>
        ///     The users preferred personal pronon.  e.g. Him
        /// </summary>
        public string Pronoun { get; set; } = default!;

        /// <summary>
        ///     The user's fullname.  e.g David Betteridge
        /// </summary>
        public string Fullname { get; set; } = default!;

        /// <summary>
        ///     The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB
        ///     (Has to be unique)
        /// </summary>
        public string DisplayName { get; set; } = default!;

        /// <summary>
        ///     The user's biography.  This can include their place of work/student,  any interests etc.
        ///     Visible to all signed in members
        /// </summary>
        public string Bio { get; set; } = default!;

        /// <summary>
        ///     The URL of the user's profile picture
        /// </summary>
        public Uri ProfilePicture { get; set; } = default!;

        /// <summary>
        ///     Is the event's host
        /// </summary>
        public bool IsHost { get; set; }

        /// <summary>
        ///     Is a speaker at the event
        /// </summary>
        public bool IsSpeaker { get; set; }

        /// <summary>
        ///     Is the user attending the event?
        /// </summary>
        public bool IsAttending { get; set; }

        /// <summary>
        ///     How many guests is this person bringing?
        /// </summary>
        public int NumberOfGuests { get; set; }

        /// <summary>
        ///     The user's dietary requirements.   Only viewable by group administrators
        /// </summary>
        public string? DietaryRequirements { get; set; }

        /// <summary>
        ///     Any information we need to gather from the user.   Only viewable by group administrators
        /// </summary>
        public Answer[] Answers { get; set; } = Array.Empty<Answer>();
    }
}
