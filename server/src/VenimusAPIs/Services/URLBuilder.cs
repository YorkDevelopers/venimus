using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using VenimusAPIs.Models;
using VenimusAPIs.Settings;

namespace VenimusAPIs.Services
{
    public class URLBuilder
    {
        private readonly SiteSettings _siteSettings;

        public URLBuilder(IOptions<SiteSettings> siteSettings)
        {
            _siteSettings = siteSettings.Value;
        }

        public Uri BuildGroupLogoURL(string groupSlug)
        {
            return new Uri($"{_siteSettings.PublicURL}api/groups/{groupSlug}/logo");
        }

        public Uri BuildCurrentUserDetailsURL()
        {
            return new Uri($"{_siteSettings.PublicURL}api/user");
        }

        internal Uri BuildUserDetailsProfilePictureURL(User theUser) => BuildUserDetailsProfilePictureURL(theUser.Id);

        internal Uri BuildUserDetailsProfilePictureURL(ObjectId theUserID)
        {
            return new Uri($"{_siteSettings.PublicURL}api/users/{theUserID.ToString()}/profilepicture");
        }
    }
}
