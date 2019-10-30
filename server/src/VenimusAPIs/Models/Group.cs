using System.Collections.Generic;
using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class Group
    {
        /// <summary>
        ///     The unique ID of this group
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Members of this group
        /// </summary>
        public List<ObjectId>[] Members { get; set; }
    }
}
