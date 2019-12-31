using System;
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
        public byte[] ProfilePicture { get; set; } = Array.Empty<byte>();

        /// <summary>
        ///     The user's social media identities
        /// </summary>
        public List<string> Identities { get; set; } = new List<string>();

        /// <summary>
        ///     Has the user completed the registration stage (ie. entered all their details)
        /// </summary>
        public bool IsRegistered { get; set; }

        /// <summary>
        ///     Has their membership been approved by an administrator?
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        ///     Has their membership been blocked by an administrator?
        /// </summary>
        public bool IsRejected { get; set; }

        /// <summary>
        ///     Who approved/rejected their membership?
        /// </summary>
        public string? ApprovedorRejectedBy { get; set; }

        /// <summary>
        ///     When were they approved/rejected?
        /// </summary>
        public DateTime? ApprovedorRejectedOnUtc { get; set; }

        /// <summary>
        ///     Unique ID sent to the approve/reject message in slack to the identify the user.
        /// </summary>
        public Guid ApprovalID { get; set; } = Guid.NewGuid();
    }
}
