namespace YorkDeveloperEvents.ViewModels
{
    public class AddGroupMember
    {
        /// <summary>
        ///     The unique ID of the user to add
        /// </summary>
        public string Slug { get; set; } = default!;

        /// <summary>
        ///     Is the user an administrator of the group?
        /// </summary>
        public bool IsAdministrator { get; set; }
    }
}
