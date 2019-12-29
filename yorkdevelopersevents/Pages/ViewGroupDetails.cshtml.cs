using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YorkDeveloperEvents.Services;
using YorkDeveloperEvents.ViewModels;

namespace YorkDeveloperEvents
{
    public class ViewGroupDetailsModel : PageModel
    {
        public GetGroup ViewModel { get; set; }
        public IEnumerable<ListEventsForGroup> PastEvents { get; private set; }
        public IEnumerable<ListEventsForGroup> FutureEvents { get; private set; }

        public async Task OnGet([FromServices] API api, string groupSlug)
        {
            ViewModel = await api.GetGroup(groupSlug);

            var allEvents = await api.ListEventsForGroup(groupSlug, includePastEvents: true, includeFutureEvents: true);
            PastEvents = allEvents.Where(e => e.EventStartsUTC <= DateTime.UtcNow).OrderByDescending(e => e.EventStartsUTC);
            FutureEvents = allEvents.Where(e => e.EventStartsUTC > DateTime.UtcNow).OrderByDescending(e => e.EventStartsUTC);
        }
    }
}