using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class ViewGroupDetailsModel : PageModel
    {
        public GetGroup ViewModel { get; set; }

        public async Task OnGet([FromServices] API api, string groupSlug)
        {
            ViewModel = await api.GetGroup(groupSlug);
        }
    }
}