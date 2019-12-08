using System;

namespace VenimusAPIs.Validation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CallerMustBeGroupMemberAttribute : Attribute
    {
        /// <summary>
        /// If the caller doesn't have access to the group then we return NotFound(404) rather than Forbid(403)
        /// </summary>
        public bool UseNotFoundRatherThanForbidden { get; set; } = false;

        /// <summary>
        /// If the caller doesn't have access to the group then we return NoContent(204) rather than Forbid(403).
        /// A typical case is when a user's group membership is being removed and their aren't currently a member of the group.
        /// </summary>
        public bool UseNoContentRatherThanForbidden { get; set; } = false;

        /// <summary>
        /// The caller doesn't need to be a member of the group if they have the System Administrator role.
        /// </summary>
        public bool CanBeSystemAdministratorInstead { get; set; } = true;
    }
}