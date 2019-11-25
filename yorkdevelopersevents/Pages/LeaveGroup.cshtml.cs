using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace YorkDeveloperEvents.Pages
{
    public class LeaveGroupModel : BasePageModel
    {
        public LeaveGroupModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<ActionResult> OnGet(string groupSlug)
        {
            var httpClient = await APIClient();
            var response = await httpClient.DeleteAsync($"api/User/Groups/{groupSlug}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect("/MyGroups");
        }
    }
}
