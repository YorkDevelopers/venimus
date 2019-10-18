using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.ViewModels
{
    public class GetGroup
    {
        /// <summary>
        /// The unique name of the group / community.  For example York Code Dojo
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// A description of the group.
        /// </summary>
        [Required]
        public string Description { get; set; }
    }
}
