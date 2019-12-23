using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class ViewEventDetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string GroupSlug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EventSlug { get; set; }

        [BindProperty]
        public GetEvent ViewModel { get; set; }

        public async Task OnGet([FromServices] API api)
        {
            ViewModel = await api.GetEvent(GroupSlug, EventSlug);
        }
    }
}