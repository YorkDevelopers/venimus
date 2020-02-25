using MongoDB.Bson;
using System.Linq;
using VenimusAPIs.Models;

namespace VenimusAPIs.Extensions
{
    public static class GroupExtensions
    {
        public static bool UserIsGroupAdministrator(this Group group, ObjectId userId) =>
            group.Members is object && group.Members.Any(m => m.UserId == userId && m.IsAdministrator);

        public static bool UserIsGroupAdministrator(this Group group, User user) =>
            group.Members is object && group.Members.Any(m => m.UserId == user.Id && m.IsAdministrator);

        public static bool UserIsGroupMember(this Group group, User user) =>
            group.Members is object && group.Members.Any(m => m.UserId == user.Id);

        public static bool UserIsApprovedGroupMember(this Group group, User user) =>
            group.Members is object && group.Members.Any(m => m.UserId == user.Id && m.IsUserApproved);
    }
}
