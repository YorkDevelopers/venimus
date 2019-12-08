using System.Collections.Generic;
using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class User
    {
        /// <summary>
        ///     The internal ID for the user.
        /// </summary>
        public ObjectId Id { get; set; }

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
        ///     The user's profile picture
        /// </summary>
        public byte[] ProfilePicture { get; set; } = new byte[0];

        /// <summary>
        ///     The user's social media identities
        /// </summary>
        public List<string> Identities { get; set; } = new List<string>();
    }
}
