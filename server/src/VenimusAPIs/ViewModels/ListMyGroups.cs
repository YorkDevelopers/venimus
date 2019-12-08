namespace VenimusAPIs.ViewModels
{
    public class ListMyGroups
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
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
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; } = default!;

        /// <summary>
        ///     The group's logo as either an URL or in Base64.
        /// </summary>
        public string Logo { get; set; } = default!;
    }
}
