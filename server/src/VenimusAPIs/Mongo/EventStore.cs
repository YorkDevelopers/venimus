using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using VenimusAPIs.Models;
using VenimusAPIs.Settings;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Mongo
{
    public class EventStore : MongoBase
    {
        public EventStore(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
        }

        internal async Task<Event> GetEvent(string groupSlug, string eventSlug)
        {
            var events = EventsCollection();
            var theEvent = await events.Find(u => u.Slug == eventSlug && u.GroupSlug == groupSlug).SingleOrDefaultAsync();

            if (theEvent != null)
            {
                if (theEvent.Members == null)
                {
                    theEvent.Members = new List<Models.Event.EventAttendees>();
                }
            }

            return theEvent;
        }

        internal async Task<ViewAllMyEventRegistrations[]> GetMyEventRegistrations(ObjectId userID)
        {
            var currentTime = DateTime.UtcNow;

            var memberMatch = Builders<Event.EventAttendees>.Filter.Eq(a => a.UserId, userID) &
                              Builders<Event.EventAttendees>.Filter.Eq(a => a.SignedUp, true);

            var filter = Builders<Event>.Filter.ElemMatch(x => x.Members, memberMatch) &
                         Builders<Event>.Filter.Gt(ent => ent.EndTimeUTC, currentTime);

            var events = EventsCollection();

            var matchingEvents = await events.Find(filter).ToListAsync();

            return matchingEvents.Select(e => new ViewAllMyEventRegistrations
            {
                EventDescription = e.Description,
                EventFinishesUTC = e.EndTimeUTC,
                EventSlug = e.Slug,
                EventStartsUTC = e.StartTimeUTC,
                EventTitle = e.Title,
                GroupName = e.GroupName,
                GroupSlug = e.GroupSlug,
            }).ToArray();
        }

        internal async Task StoreEvent(Event newEvent)
        {
            var events = EventsCollection();

            await events.InsertOneAsync(newEvent);
        }

        internal async Task UpdateGroupDetailsInEvents(Group group)
        {
            var events = EventsCollection();

            var filter = Builders<Event>.Filter.Eq(ent => ent.GroupId, group.Id);
            var update = Builders<Event>.Update
                                        .Set(e => e.GroupName, group.Name)
                                        .Set(e => e.GroupSlug, group.Slug);

            await events.UpdateManyAsync(filter, update);
        }

        internal async Task<bool> DoEventsExistForGroup(string groupSlug)
        {
            var events = EventsCollection();

            var filter = Builders<Event>.Filter.Eq(ent => ent.GroupSlug, groupSlug);
            var eventsExist = await events.Find(filter).AnyAsync();

            return eventsExist;
        }

        internal async Task UpdateEvent(Models.Event amendedEvent)
        {
            var events = EventsCollection();

            await events.ReplaceOneAsync(u => u.Id == amendedEvent.Id, amendedEvent);
        }

        internal async Task DeleteEvent(Event theEvent)
        {
            var events = EventsCollection();

            await events.DeleteOneAsync(e => e.Id == theEvent.Id);
        }
    }
}
