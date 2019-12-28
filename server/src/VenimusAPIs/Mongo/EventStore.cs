using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VenimusAPIs.Models;
using VenimusAPIs.Services;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Mongo
{
    public class EventStore
    {
        private readonly MongoConnection _mongoConnection;
        private readonly URLBuilder _groupLogoURLBuilder;

        public EventStore(MongoConnection mongoConnection, URLBuilder groupLogoURLBuilder)
        {
            _mongoConnection = mongoConnection;
            _groupLogoURLBuilder = groupLogoURLBuilder;
        }

        internal async Task<GroupEvent?> GetEvent(string groupSlug, string eventSlug)
        {
            var events = _mongoConnection.EventsCollection();
            var theEvent = await events.Find(u => u.Slug == eventSlug && u.GroupSlug == groupSlug).SingleOrDefaultAsync().ConfigureAwait(false);

            if (theEvent != null)
            {
                if (theEvent.Members == null)
                {
                    theEvent.Members = new List<Models.GroupEventAttendee>();
                }
            }

            return theEvent;
        }

        internal async Task<List<ListEvents>> GetMyEventRegistrations(ObjectId userID)
        {
            var currentTime = DateTime.UtcNow;

            var memberMatch = Builders<GroupEventAttendee>.Filter.Eq(a => a.UserId, userID) &
                              Builders<GroupEventAttendee>.Filter.Eq(a => a.SignedUp, true);

            var filter = Builders<GroupEvent>.Filter.ElemMatch(x => x.Members, memberMatch) &
                         Builders<GroupEvent>.Filter.Gt(ent => ent.EndTimeUTC, currentTime);

            var events = _mongoConnection.EventsCollection();

            var matchingEvents = await events.Find(filter).ToListAsync().ConfigureAwait(false);

            return matchingEvents.Select(e => new ListEvents
            {
                EventDescription = e.Description,
                EventFinishesUTC = e.EndTimeUTC,
                EventSlug = e.Slug,
                EventStartsUTC = e.StartTimeUTC,
                EventTitle = e.Title,
                GroupName = e.GroupName,
                GroupSlug = e.GroupSlug,
                GroupLogo = _groupLogoURLBuilder.BuildGroupLogoURL(e.GroupSlug).ToString(),
            }).ToList();
        }

        internal async Task UpdateUserDetailsInEvents(User user)
        {
            var events = _mongoConnection.EventsCollection();

            var memberMatch = Builders<GroupEventAttendee>.Filter.Eq(a => a.UserId, user.Id);
            var filter = Builders<GroupEvent>.Filter.ElemMatch(x => x.Members, memberMatch);
            var eventsTheUserBelongsTo = await events.Find(filter).ToListAsync().ConfigureAwait(false);

            foreach (var evt in eventsTheUserBelongsTo)
            {
                var member = evt.Members.Single(g => g.UserId == user.Id);
                member.Bio = user.Bio;
                member.DisplayName = user.DisplayName;
                member.EmailAddress = user.EmailAddress;
                member.Fullname = user.Fullname;
                member.Pronoun = user.Pronoun;
                await events.ReplaceOneAsync(u => u.Id == evt.Id, evt).ConfigureAwait(false);
            }
        }

        internal async Task StoreEvent(GroupEvent newEvent)
        {
            var events = _mongoConnection.EventsCollection();

            await events.InsertOneAsync(newEvent).ConfigureAwait(false);
        }

        internal async Task UpdateGroupDetailsInEvents(Group group)
        {
            var events = _mongoConnection.EventsCollection();

            var filter = Builders<GroupEvent>.Filter.Eq(ent => ent.GroupId, group.Id);
            var update = Builders<GroupEvent>.Update
                                        .Set(e => e.GroupName, group.Name)
                                        .Set(e => e.GroupSlug, group.Slug);

            await events.UpdateManyAsync(filter, update).ConfigureAwait(false);
        }

        internal async Task<bool> DoEventsExistForGroup(string groupSlug)
        {
            var events = _mongoConnection.EventsCollection();

            var filter = Builders<GroupEvent>.Filter.Eq(ent => ent.GroupSlug, groupSlug);
            var eventsExist = await events.Find(filter).AnyAsync().ConfigureAwait(false);

            return eventsExist;
        }

        internal async Task UpdateEvent(Models.GroupEvent amendedEvent)
        {
            var events = _mongoConnection.EventsCollection();

            await events.ReplaceOneAsync(u => u.Id == amendedEvent.Id, amendedEvent).ConfigureAwait(false);
        }

        internal async Task DeleteEvent(GroupEvent theEvent)
        {
            var events = _mongoConnection.EventsCollection();

            await events.DeleteOneAsync(e => e.Id == theEvent.Id).ConfigureAwait(false);
        }
    }
}
