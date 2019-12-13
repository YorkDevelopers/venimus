namespace VenimusAPIs.ViewModels
{
    public class Question
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
        ///     Type of question,  text, date, boolean etc
        /// </summary>
        public string QuestionType { get; set; } = default!;
    }
}
