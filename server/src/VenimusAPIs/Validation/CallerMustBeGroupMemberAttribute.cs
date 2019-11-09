using System;

namespace VenimusAPIs.Validation
{
    public class CallerMustBeGroupMemberAttribute : Attribute
    {
        public bool UseNotFoundRatherThanForbidden { get; set; }

        public CallerMustBeGroupMemberAttribute()
        {
        }
    }
}