using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VenimusAPIs.Validation
{
    public class CheckGroupSecurityFilter : IAsyncActionFilter
    {
        private readonly Services.Mongo _mongo;

        public CheckGroupSecurityFilter(Services.Mongo mongo)
        {
            _mongo = mongo;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            var mustBeAGroupAdministrator = RequiresGroupAdministrator(context.ActionDescriptor);
            var mustBeAGroupMember = RequiresGroupMembership(context.ActionDescriptor, out var callerMustBeGroupMemberAttribute);

            var canBeSystemAdministratorInstead = true;
            var useNotFoundRatherThanForbidden = false;
            var useNoContentRatherThanForbidden = false;
            if (mustBeAGroupMember)
            {
                canBeSystemAdministratorInstead = callerMustBeGroupMemberAttribute.CanBeSystemAdministratorInstead;
                useNotFoundRatherThanForbidden = callerMustBeGroupMemberAttribute.UseNotFoundRatherThanForbidden;
                useNoContentRatherThanForbidden = callerMustBeGroupMemberAttribute.UseNoContentRatherThanForbidden;
            }

            if (mustBeAGroupAdministrator || mustBeAGroupMember)
            {
                if (!canBeSystemAdministratorInstead || !user.IsInRole("SystemAdministrator"))
                {
                    var groupSlug = context.ActionArguments["groupSlug"].ToString();

                    var uniqueID = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                    var existingUser = await _mongo.GetUserByID(uniqueID);
                    if (existingUser == null)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }

                    var group = await _mongo.RetrieveGroupBySlug(groupSlug);
                    if (group == null)
                    {
                        context.Result = new NotFoundResult();
                        return;
                    }

                    if (group.Members == null || !group.Members.Any(m => m.Id == existingUser.Id && (!mustBeAGroupAdministrator || m.IsAdministrator)))
                    {
                        if (useNotFoundRatherThanForbidden)
                        {
                            context.Result = new NotFoundResult();
                        }
                        else if (useNoContentRatherThanForbidden)
                        {
                            context.Result = new NoContentResult();
                        }
                        else
                        {
                            context.Result = new ForbidResult();
                        }

                        return;
                    }
                }
            }

            await next();
        }

        private static bool RequiresGroupAdministrator(ActionDescriptor actionDescriptor)
        {
            var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                var attribute = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(CallerMustBeGroupAdministratorAttribute), true).SingleOrDefault();
                return attribute != null;
            }

            return false;
        }

        private static bool RequiresGroupMembership(ActionDescriptor actionDescriptor, out CallerMustBeGroupMemberAttribute attribute)
        {
            var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                attribute = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(CallerMustBeGroupMemberAttribute), true).SingleOrDefault() as CallerMustBeGroupMemberAttribute;
                if (attribute != null)
                {
                    return true;
                }
            }

            attribute = null;
            return false;
        }
    }
}