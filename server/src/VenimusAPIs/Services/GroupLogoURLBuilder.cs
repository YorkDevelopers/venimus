using Microsoft.AspNetCore.Http;

namespace VenimusAPIs.Services
{
    public class GroupLogoURLBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupLogoURLBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string BuildURL(string groupSlug)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var scheme = request.IsHttps ? "https" : "http";
            var server = $"{scheme}://{request.Host}";

            return $"{server}/api/groups/{groupSlug}/logo";
        }
    }
}
