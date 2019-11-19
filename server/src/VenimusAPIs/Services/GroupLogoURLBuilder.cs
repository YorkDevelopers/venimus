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
            var server = $"{request.Scheme}://{request.Host}";

            return $"{server}/api/groups/{groupSlug}/logo";
        }
    }
}
