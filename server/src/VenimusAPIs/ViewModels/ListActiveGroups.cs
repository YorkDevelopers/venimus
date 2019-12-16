﻿using System;

namespace VenimusAPIs.ViewModels
{
    public class ListActiveGroups
    {
        /// <summary>
        ///     The unique external code for the group.  For example YorkCodeDojo
        /// </summary>
        public string Slug { get; set; } = default!;

        /// <summary>
        /// The unique name for the group / community.  For example York Code Dojo
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// A description of the group in markdown
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// A very short one-line description of the group
        /// </summary>
        public string StrapLine { get; set; } = default!;

        /// <summary>
        ///     The URL to retrieve the groups logo.
        /// </summary>
        public Uri Logo { get; set; } = default!;
    }
}
