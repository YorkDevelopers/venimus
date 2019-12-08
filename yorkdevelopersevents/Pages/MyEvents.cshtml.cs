using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class MyEventsModel : PageModel
    {
        public ViewAllMyEventRegistrations[] ViewModel { get; set; }

        public async Task OnGet([FromServices] API api)
        {
            ViewModel = await api.ListMyEvents();
        }
    }
}