using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class ViewEventAttendeesModel : PageModel
    {
        public ListEventAttendees[] ViewModel { get; set; }

        public async Task OnGet([FromServices] API api, string groupSlug, string eventSlug)
        {
            ViewModel = await api.ListEventAttendees(groupSlug, eventSlug);
        }
    }
}