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
        public string Slug { get; set; } = default!;

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
        ///     The group's logo.
        /// </summary>
        public byte[] Logo { get; set; } = default!;

        /// <summary>
        /// Members of this group
        /// </summary>
        public List<GroupMember> Members { get; set; } = new List<GroupMember>();

        /// <summary>
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; } = default!;
    }
}
