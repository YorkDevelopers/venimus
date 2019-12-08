using System.ComponentModel.DataAnnotations;
using VenimusAPIs.Validation;

namespace VenimusAPIs.ViewModels
{
    public class UpdateGroup
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Slug]
        public string Slug { get; set; } = default!;

        /// <summary>
        ///     Is this group still actively running events?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// A description of the group in markdown
        /// </summary>
        [Required]
        public string Description { get; set; } = default!;

        /// <summary>
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; } = default!;

        /// <summary>
        ///     The group's logo.
        /// </summary>
        public string LogoInBase64 { get; set; } = default!;
    }
}
