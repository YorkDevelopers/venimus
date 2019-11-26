using System.Net.Http;
using System.Threading.Tasks;
using YorkDeveloperEvents.Extensions;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents.Pages
{
    public class ViewGroupMembersModel : BasePageModel
    {
        public ViewGroupMembersModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public ListGroupMembers[] ViewModel { get; set; }

        public async Task OnGet(string groupSlug)
        {
            var httpClient = await APIClient();
            ViewModel = await httpClient.GetAsJson<ListGroupMembers[]>($"/api/Groups/{groupSlug}/Members");
        }
    }
}