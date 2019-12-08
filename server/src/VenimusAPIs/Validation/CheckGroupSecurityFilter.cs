using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VenimusAPIs.Validation
{
    public class CheckGroupSecurityFilter : IAsyncActionFilter
    {
        private readonly Mongo.GroupStore _groupStore;
        private readonly Mongo.UserStore _userStore;

        public CheckGroupSecurityFilter(Mongo.GroupStore groupStore, Mongo.UserStore userStore)
        {
            _groupStore = groupStore;
            _userStore = userStore;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            var mustBeAGroupAdministrator = HasAttribute<CallerMustBeGroupAdministratorAttribute>(context.ActionDescriptor, out var callerMustBeGroupAdministratorAttribute);
            var mustBeAGroupMember = HasAttribute<CallerMustBeGroupMemberAttribute>(context.ActionDescriptor, out var callerMustBeGroupMemberAttribute);
            var mustBeAnApprovedGroupMember = HasAttribute<CallerMustBeApprovedGroupMemberAttribute>(context.ActionDescriptor, out var callerMustBeApprovedGroupMemberAttribute);

            var canBeSystemAdministratorInstead = true;
            var useNotFoundRatherThanForbidden = false;
            var useNoContentRatherThanForbidden = false;
            if (mustBeAGroupMember && callerMustBeGroupMemberAttribute != null)
            {
                canBeSystemAdministratorInstead = callerMustBeGroupMemberAttribute.CanBeSystemAdministratorInstead;
                useNotFoundRatherThanForbidden = callerMustBeGroupMemberAttribute.UseNotFoundRatherThanForbidden;
                useNoContentRatherThanForbidden = callerMustBeGroupMemberAttribute.UseNoContentRatherThanForbidden;
            }
            else if (mustBeAnApprovedGroupMember && callerMustBeApprovedGroupMemberAttribute != null)
            {
                canBeSystemAdministratorInstead = callerMustBeApprovedGroupMemberAttribute.CanBeSystemAdministratorInstead;
            }

            if (mustBeAGroupAdministrator || mustBeAGroupMember || mustBeAnApprovedGroupMember)
            {
                if (!canBeSystemAdministratorInstead || !user.IsInRole("SystemAdministrator"))
                {
                    var groupSlug = context.ActionArguments["groupSlug"].ToString();
                    if (groupSlug == null)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }

                    var uniqueID = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                    var existingUser = await _userStore.GetUserByID(uniqueID).ConfigureAwait(false);
                    if (existingUser == null)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }

                    var group = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);
                    if (group == null)
                    {
                        context.Result = new NotFoundResult();
                        return;
                    }

                    if (group.Members == null || !group.Members.Any(m => m.UserId == existingUser.Id
                                                                    && (!mustBeAGroupAdministrator || m.IsAdministrator)
                                                                    && (!mustBeAnApprovedGroupMember || m.IsApproved)))
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

            await next().ConfigureAwait(false);
        }

        private static bool HasAttribute<T>(ActionDescriptor actionDescriptor, out T? attribute)
            where T : Attribute
        {
            if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                attribute = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(T), true).SingleOrDefault() as T;
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