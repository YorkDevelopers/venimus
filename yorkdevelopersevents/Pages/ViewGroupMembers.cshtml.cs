using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents.Pages
{
    [Authorize]
    public class ViewGroupMembersModel : PageModel
    {
        public ListGroupMembers[] ViewModel { get; set; }

        public async Task OnGet([FromServices] API api, string groupSlug)
        {
            ViewModel = await api.ListGroupMembers(groupSlug);
        }
    }
}