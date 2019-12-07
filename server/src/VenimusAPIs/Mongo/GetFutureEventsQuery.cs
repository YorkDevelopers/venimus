using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.Services;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Mongo
{
    public class GetFutureEventsQuery
    {
        private readonly MongoConnection _mongoConnection;
        private readonly URLBuilder _groupLogoURLBuilder;

        public GetFutureEventsQuery(MongoConnection mongoConnection, URLBuilder groupLogoURLBuilder)
        {
            _mongoConnection = mongoConnection;
            _groupLogoURLBuilder = groupLogoURLBuilder;
        }

        internal async Task<List<ViewModels.ListFutureEvents>> Evaluate()
        {
            var database = _mongoConnection.ConnectToDatabase();

            var events = database.GetCollection<Models.Event>("events");
            var groups = database.GetCollection<Models.Group>("groups");

            var currentTime = DateTime.UtcNow;

            var allGroups = await groups.FindAsync(Builders<Group>.Filter.Empty);
            var groupsList = await allGroups.ToListAsync();

            var result = new List<ViewModels.ListFutureEvents>();
            foreach (var group in groupsList)
            {
                var filter = Builders<Event>.Filter.Eq(ent => ent.GroupId, group.Id) &
                             Builders<Event>.Filter.Gt(ent => ent.StartTimeUTC, currentTime);

                var sort = Builders<Event>.Sort.Ascending("StartTime");

                var nextEvents = await events.FindAsync(filter, new FindOptions<Event, Event>()
                {
                    Limit = 10,
                    Sort = sort,
                });

                var viewModels = nextEvents.ToEnumerable().Select(e => new ListFutureEvents
                {
                    EventDescription = e.Description,
                    EventFinishesUTC = e.EndTimeUTC,
                    EventSlug = e.Slug,
                    EventStartsUTC = e.StartTimeUTC,
                    EventTitle = e.Title,
                    GroupName = e.GroupName,
                    GroupLogo = _groupLogoURLBuilder.BuildGroupLogoURL(e.GroupSlug),
                    GroupSlug = e.GroupSlug,
                });

                result.AddRange(viewModels);
            }

            return result;
        }
    }
}
