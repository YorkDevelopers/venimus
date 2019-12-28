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

        public static void AddAnswer(this Models.GroupEventAttendee groupEventAttendee, string code, string userAnswer)
        {
            var answer = new Models.Answer()
            {
                Code = code,
                UsersAnswer = userAnswer,
            };

            groupEventAttendee.Answers.Add(answer);
        }
    }
}
