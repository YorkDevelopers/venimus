namespace VenimusAPIs.ViewModels
{
    public class ViewMyGroupMembership
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
        /// </summary>
        public string GroupSlug { get; set; }

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// A description of the group in markdown
        /// </summary>
        public string GroupDescription { get; set; }

        /// <summary>
        ///     The name of this groups slack channel
        /// </summary>
        public string GroupSlackChannelName { get; set; }

        /// <summary>
        ///     The group's logo.
        /// </summary>
        public string GroupLogoInBase64 { get; set; }
    }
}
