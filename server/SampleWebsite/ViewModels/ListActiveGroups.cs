namespace YorkDeveloperEvents.ViewModels
{
    public class ListActiveGroups
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        public string Name { get; set; }

        public string name { get; set; }

        /// <summary>
        /// A description of the group in markdown
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The group's logo. (Either URL or Base64)
        /// </summary>
        public string Logo { get; set; }

        public string logo { get; set; }
    }
}
