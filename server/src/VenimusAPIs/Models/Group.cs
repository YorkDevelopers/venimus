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
        public List<ObjectId> Members { get; set; }

        /// <summary>
        /// Administrators of this group
        /// </summary>
        public List<ObjectId> Administrators { get; set; }

        /// <summary>
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; }
    }
}
