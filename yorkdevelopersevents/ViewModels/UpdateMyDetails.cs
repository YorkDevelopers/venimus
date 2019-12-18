using System.ComponentModel.DataAnnotations;

namespace YorkDeveloperEvents.ViewModels
{
    public class UpdateMyDetails
    {
        /// <summary>
        ///     The users preferred personal pronon.  e.g. Him
        /// </summary>
        public string Pronoun { get; set; }

        /// <summary>
        ///     The user's fullname.  e.g David Betteridge
        /// </summary>
        [Required]
        public string Fullname { get; set; }

        /// <summary>
        ///     The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB
        ///     (Has to be unique)
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        ///     The user's biography.  This can include their place of work/student,  any interests etc.
        ///     Visible to all signed in members
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        ///     The user's profile picture in base64
        /// </summary>
        public string ProfilePictureAsBase64 { get; set; }

        /// <summary>
        ///     Has the user completed the registration stage (ie. entered all their details)
        /// </summary>
        public bool IsRegistered { get; set; }
    }
}
