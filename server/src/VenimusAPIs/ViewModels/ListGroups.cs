namespace VenimusAPIs.ViewModels
{
    public class ListGroups
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
        ///     The group's logo, either a URL or Base64 data
        /// </summary>
        public string Logo { get; set; } = default!;
    }
}
