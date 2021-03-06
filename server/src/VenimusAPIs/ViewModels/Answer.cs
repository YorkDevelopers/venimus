﻿namespace VenimusAPIs.ViewModels
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
        public string? Caption { get; set; }

        /// <summary>
        ///     Type of question,  text, date, boolean etc
        /// </summary>
        public string? QuestionType { get; set; }

        /// <summary>
        ///     Answer from the user
        /// </summary>
        public string UsersAnswer { get; set; } = default!;
    }
}
