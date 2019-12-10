namespace YorkDeveloperEvents.ViewModels
{
    public class ListMyGroups
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
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
        ///     The name of this groups slack channel
        /// </summary>
        public string SlackChannelName { get; set; }

        /// <summary>
        ///     The group's logo as either an URL or in Base64.
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        ///     Can this user view the list of members in this group?
        /// </summary>
        public bool CanViewMembers { get; set; }
    }
}
