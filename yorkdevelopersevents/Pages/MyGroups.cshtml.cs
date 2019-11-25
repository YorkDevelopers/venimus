using System.Net.Http;
using System.Threading.Tasks;
using YorkDeveloperEvents.Extensions;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents.Pages
{
    public class MyGroupsModel : BasePageModel
    {
        public MyGroupsModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public ListMyGroups[] ViewModel { get; set; }

        public async Task OnGet()
        {
            var httpClient = await APIClient();
            ViewModel = await httpClient.GetAsJson<ListMyGroups[]>("/api/user/groups");
        }
    }
}