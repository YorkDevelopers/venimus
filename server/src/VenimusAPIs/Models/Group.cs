using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class Group
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Required for Mongo")]
        public ObjectId _id { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the group.
        /// </summary>
        public string Description { get; set; }
    }
}
