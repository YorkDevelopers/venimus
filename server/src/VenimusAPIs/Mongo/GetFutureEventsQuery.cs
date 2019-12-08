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

            var events = database.GetCollection<Models.GroupEvent>("events");
            var groups = database.GetCollection<Models.Group>("groups");

            var currentTime = DateTime.UtcNow;

            var allGroups = await groups.FindAsync(Builders<Group>.Filter.Empty).ConfigureAwait(false);
            var groupsList = await allGroups.ToListAsync().ConfigureAwait(false);

            var result = new List<ViewModels.ListFutureEvents>();
            foreach (var group in groupsList)
            {
                var filter = Builders<GroupEvent>.Filter.Eq(ent => ent.GroupId, group.Id) &
                             Builders<GroupEvent>.Filter.Gt(ent => ent.StartTimeUTC, currentTime);

                var sort = Builders<GroupEvent>.Sort.Ascending("StartTime");

                var nextEvents = await events.FindAsync(filter, new FindOptions<GroupEvent, GroupEvent>()
                {
                    Limit = 10,
                    Sort = sort,
                }).ConfigureAwait(false);

                var viewModels = nextEvents.ToEnumerable().Select(e => new ListFutureEvents
                {
                    EventDescription = e.Description,
                    EventFinishesUTC = e.EndTimeUTC,
                    EventSlug = e.Slug,
                    EventStartsUTC = e.StartTimeUTC,
                    EventTitle = e.Title,
                    GroupName = e.GroupName,
                    GroupLogo = _groupLogoURLBuilder.BuildGroupLogoURL(e.GroupSlug).ToString(),
                    GroupSlug = e.GroupSlug,
                });

                result.AddRange(viewModels);
            }

            return result;
        }
    }
}
