namespace VenimusAPIs.ViewModels
{
    public class ListGroupMembers
    {
        /// <summary>
        ///     The external ID for the user.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        ///     The email address which also links all the social media accounts together.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     The users preferred personal pronon.  e.g. Him
        /// </summary>
        public string Pronoun { get; set; }

        /// <summary>
        ///     The user's fullname.  e.g David Betteridge
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        ///     The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB
        ///     (Has to be unique)
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     The user's biography.  This can include their place of work/student,  any interests etc.
        ///     Visible to all signed in members
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        ///     The user's profile picture
        /// </summary>
        public string ProfilePictureInBase64 { get; set; }

        /// <summary>
        ///     Is the user a group administrator?
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        ///     Has this user's membership to the group been approved?
        /// </summary>
        public bool IsApproved { get; set; }
    }
}
