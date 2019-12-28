namespace VenimusAPIs.Models
{
    public class Answer
    {
        /// <summary>
        ///     Unique ID for this question.
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        ///     Question shown to the user
        /// </summary>
        public string Caption { get; set; } = default!;

        /// <summary>
        ///     The user's answer to the question
        /// </summary>
        public string UsersAnswer { get; set; } = default!;
    }
}
