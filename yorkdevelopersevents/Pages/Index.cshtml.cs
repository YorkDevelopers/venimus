using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents.Pages
{
    public class IndexModel : PageModel
    {
        public ListGroups[] ViewModel { get; set; }

        public async Task OnGet([FromServices] API api)
        {
            ViewModel = await api.ListGroups(includeInActiveGroups: false);
        }
    }
}
