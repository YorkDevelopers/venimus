using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class Group
    {
        /// <summary>
        ///     The unique internal ID of this group
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        ///     Is this group still actively running events?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     The unique external id for the group.  For example YorkCodeDojo
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the group in markdown
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The group's logo.
        /// </summary>
        public byte[] Logo { get; set; }

        /// <summary>
        /// Members of this group
        /// </summary>
        public List<GroupMember> Members { get; set; }

        /// <summary>
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; }

        public class GroupMember
        {
            /// <summary>
            ///     Unique ID of the user
            /// </summary>
            public ObjectId UserId { get; set; } 

            /// <summary>
            ///     Are they an administrator of the group?
            /// </summary>
            public bool IsAdministrator { get; set; }

            /// <summary>
            ///     The email address which also links all the social media accounts together.
            /// </summary>
            public string EmailAddress { get; set; }

            /// <summary>
            ///     The users preferred personal pronon.  e.g. Him
            /// </summary>
            public string Pronoun { get; set; }

            /// <summary>
            ///     The user's fullname.  e.g David Betteridge
            /// </summary>
            public string Fullname { get; set; }

            /// <summary>
            ///     The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB
            ///     (Has to be unique)
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            ///     The user's biography.  This can include their place of work/student,  any interests etc.
            ///     Visible to all signed in members
            /// </summary>
            public string Bio { get; set; }

            /// <summary>
            ///     Has their membership been approved by the group administrator?
            /// </summary>
            public bool IsApproved { get; set; }
        }
    }
}
