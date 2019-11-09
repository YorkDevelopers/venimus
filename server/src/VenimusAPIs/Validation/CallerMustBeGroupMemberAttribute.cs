using System;

namespace VenimusAPIs.Validation
{
    public class CallerMustBeGroupMemberAttribute : Attribute
    {
        /// <summary>
        /// If the caller doesn't have access to the group then we return NotFound(404) rather than Forbid(403)
        /// </summary>
        public bool UseNotFoundRatherThanForbidden { get; set; } = false;

        /// <summary>
        /// The caller doesn't need to be a member of the group if they have the System Administrator role.
        /// </summary>
        public bool CanBeSystemAdministratorInstead { get; set; } = true;
    }
}