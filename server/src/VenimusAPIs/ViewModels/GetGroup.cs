﻿using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.ViewModels
{
    public class GetGroup
    {
        /// <summary>
        /// The unique name of the group / community.  For example York Code Dojo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the group.
        /// </summary>
        public string Description { get; set; }
    }
}
