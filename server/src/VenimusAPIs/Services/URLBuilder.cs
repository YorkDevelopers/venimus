using System;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using VenimusAPIs.Models;

namespace VenimusAPIs.Services
{
    public class URLBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public URLBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Uri BuildGroupLogoURL(string groupSlug)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var server = $"{request.Scheme}://{request.Host}";

            return new Uri($"{server}/api/groups/{groupSlug}/logo");
        }

        public Uri BuildCurrentUserDetailsURL()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var server = $"{request.Scheme}://{request.Host}";

            return new Uri($"{server}/api/user");
        }

        internal string BuildUserDetailsProfilePictureURL(User theUser) => BuildUserDetailsProfilePictureURL(theUser.Id);

        internal string BuildUserDetailsProfilePictureURL(ObjectId theUserID)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var server = $"{request.Scheme}://{request.Host}";

            return $"{server}/api/users/{theUserID.ToString()}/profilepicture";
        }
    }
}
