using System;

namespace VenimusAPIs.Validation
{
    public class CallerMustBeApprovedGroupMemberAttribute : Attribute
    {
        /// <summary>
        /// The caller doesn't need to be a member of the group if they have the System Administrator role.
        /// </summary>
        public bool CanBeSystemAdministratorInstead { get; set; } = true;
    }
}