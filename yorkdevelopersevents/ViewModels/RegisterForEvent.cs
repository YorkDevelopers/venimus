using System;

namespace YorkDeveloperEvents.ViewModels
{
    public class RegisterForEvent
    {
        /// <summary>
        ///     The user's answers
        /// </summary>
        /// <returns></returns>
        public Answer[] Answers { get; set; } = Array.Empty<Answer>();
    }
}
