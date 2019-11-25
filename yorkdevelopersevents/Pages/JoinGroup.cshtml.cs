using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using YorkDeveloperEvents.Extensions;

namespace YorkDeveloperEvents.Pages
{
    public class JoinGroupModel : BasePageModel
    {
        public JoinGroupModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<ActionResult> OnGet(string groupSlug)
        {
            var httpClient = await APIClient();

            var data = new { GroupSlug = groupSlug };
            var response = await httpClient.PostAsJsonAsync($"api/User/Groups", data);

            response.EnsureSuccessStatusCode();

            return LocalRedirect("/MyGroups");
        }
    }
}