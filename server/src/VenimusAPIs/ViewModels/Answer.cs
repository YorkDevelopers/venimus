namespace VenimusAPIs.ViewModels
{
    public class Answer
    {
        /// <summary>
        ///     Unique ID for this question.
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        ///     Answer from the user
        /// </summary>
        public string UsersAnswer { get; set; } = default!;
    }
}
