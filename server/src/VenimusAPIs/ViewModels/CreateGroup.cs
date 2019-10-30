using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.ViewModels
{
    public class CreateGroup
    {
        /// <summary>
        /// The unique name for the group / community.  For example YorkCodeDojo
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
