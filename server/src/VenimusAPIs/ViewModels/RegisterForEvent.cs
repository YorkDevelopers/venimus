﻿using System.Collections.Generic;

namespace VenimusAPIs.ViewModels
{
    public class RegisterForEvent
    {
        /// <summary>
        ///     The user's answers
        /// </summary>
        /// <returns></returns>
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
}
