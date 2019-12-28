using MongoDB.Bson;
using System.Collections.Generic;

namespace VenimusAPIs.Models
{
    public class GroupEventAttendees
    {
        public ObjectId UserId { get; set; }

        /// <summary>
        ///     Are they still signed up
        /// </summary>
        public bool SignedUp { get; set; }

        /// <summary>
        ///     Did they attend the event?
        /// </summary>
        public bool? Attended { get; set; }

        /// <summary>
        ///     Are they organising the event
        /// </summary>
        public bool Host { get; set; }

        /// <summary>
        ///     Are they presenting at the event
        /// </summary>
        public bool Speaker { get; set; }

        /// <summary>
        ///     Any Dietary Requirements the user has
        /// </summary>
        public string DietaryRequirements { get; set; } = default!;

        /// <summary>
        ///     The number of unregistered guest the person is bringing
        /// </summary>
        public int NumberOfGuests { get; set; }

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
        ///     Answers to any custom questions.  Does not include number of guests or dietary requirements
        /// </summary>
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
}
