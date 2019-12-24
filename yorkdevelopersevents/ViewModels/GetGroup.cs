using System;

namespace YorkDeveloperEvents.ViewModels
{
    public class GetGroup
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
        /// </summary>
        public string Slug { get; set; } = default!;

        /// <summary>
        ///     Is this group still actively running events?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// A description of the group in markdown
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// A very short one-line description of the group
        /// </summary>
        public string StrapLine { get; set; } = default!;

        /// <summary>
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; } = default!;

        /// <summary>
        ///     The group's logo.
        /// </summary>
        public Uri Logo { get; set; } = default!;

        /// <summary>
        ///     Can this user view the list of members in this group?
        /// </summary>
        public bool CanViewMembers { get; set; }

        /// <summary>
        ///     Can this user create new events for this group?
        /// </summary>
        public bool CanAddEvents { get; set; }

        /// <summary>
        ///     Is the current user allowed to add members to this group?
        /// </summary>
        public bool CanAddMembers { get; set; }

        /// <summary>
        ///     Can the user join this group?
        /// </summary>
        public bool CanJoinGroup { get; set; }

        /// <summary>
        ///     Can the user leave the group?
        /// </summary>
        public bool CanLeaveGroup { get; set; }

        /// <summary>
        ///     Can the user edit the details of group?
        /// </summary>
        public bool CanEditGroup { get; set; }
    }
}
