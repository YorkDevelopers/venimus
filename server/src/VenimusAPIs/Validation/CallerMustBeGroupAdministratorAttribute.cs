using System;

namespace VenimusAPIs.Validation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CallerMustBeGroupAdministratorAttribute : Attribute
    {
    }
}