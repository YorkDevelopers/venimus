using System.ComponentModel.DataAnnotations;

namespace YorkDeveloperEvents.ViewModels
{
    public class CreateGroup
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; }

        /// <summary>
        ///     Is this group still actively running events?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// A description of the group in markdown
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; }

        /// <summary>
        ///     The group's logo.
        /// </summary>
        public string LogoInBase64 { get; set; }
    }
}
