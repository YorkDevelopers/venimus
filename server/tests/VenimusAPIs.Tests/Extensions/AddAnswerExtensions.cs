using System.Collections.Generic;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Tests.Extensions
{
    public static class AddAnswerExtensions
    {
        public static void AddAnswer(this ViewModels.RegisterForEvent amendedDetails, string code, string userAnswer)
        {
            var answer = new Answer()
            {
                Code = code,
                UsersAnswer = userAnswer,
            };

            if (amendedDetails.Answers == null)
                amendedDetails.Answers = new List<Answer>();

            amendedDetails.Answers.Add(answer);
        }
    }
}
