using Microsoft.AspNetCore.Http;

namespace VenimusAPIs.Services
{
    public class URLBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public URLBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string BuildGroupLogoURL(string groupSlug)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var server = $"{request.Scheme}://{request.Host}";

            return $"{server}/api/groups/{groupSlug}/logo";
        }

        public string BuildCurrentUserDetailsURL()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var server = $"{request.Scheme}://{request.Host}";

            return $"{server}/api/user";
        }
    }
}
