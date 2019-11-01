using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using VenimusAPIs.Models;
using VenimusAPIs.ViewModels;

namespace VenimusAPIs.Services
{
    public class Mongo
    {
        private readonly Settings.MongoDBSettings _mongoDBSettings;

        public Mongo(IOptions<Settings.MongoDBSettings> mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings.Value;
        }

        private IMongoCollection<Group> GroupsCollection()
        {
            var database = ConnectToDatabase();
            var groups = database.GetCollection<Models.Group>("groups");
            return groups;
        }

        private IMongoCollection<Event> EventsCollection()
        {
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.Event>("events");
            return events;
        }

        private IMongoCollection<User> UsersCollection()
        {
            var database = ConnectToDatabase();
            var users = database.GetCollection<Models.User>("users");
            return users;
        }

        internal async Task<List<Models.Group>> GetActiveGroups()
        {
            var groups = GroupsCollection();
            var models = await groups.Find(u => u.IsActive).ToListAsync();

            return models;
        }

        public async Task StoreGroup(Models.Group group)
        {
            var groups = GroupsCollection();

            await groups.InsertOneAsync(group);
        }

        public async Task<Models.Group> RetrieveGroup(string groupSlug)
        {
            var groups = GroupsCollection();
            var group = await groups.Find(u => u.Slug == groupSlug).SingleOrDefaultAsync();

            return group;
        }

        public async Task<List<Models.Group>> RetrieveAllGroups()
        {
            var groups = GroupsCollection();
            var models = await groups.Find(u => true).ToListAsync();

            return models;
        }

        internal async Task StoreEvent(Event newEvent)
        {
            var events = EventsCollection();

            await events.InsertOneAsync(newEvent);
        }

        internal async Task<List<ViewModels.ListFutureEvents>> GetFutureEvents()
        {
            var database = ConnectToDatabase();

            var events = database.GetCollection<Models.Event>("events");
            var groups = database.GetCollection<Models.Group>("groups");

            var currentTime = DateTime.UtcNow;
            
            var allGroups = await groups.FindAsync(Builders<Group>.Filter.Empty);
            var groupsList = await allGroups.ToListAsync();

            var result = new List<ViewModels.ListFutureEvents>();
            foreach (var group in groupsList)
            {
                var filter = Builders<Event>.Filter.Eq(ent => ent.GroupId, group.Id) &
                             Builders<Event>.Filter.Gt(ent => ent.StartTime, currentTime);

                var sort = Builders<Event>.Sort.Ascending("StartTime");

                var nextEvents = await events.FindAsync(filter, new FindOptions<Event, Event>()
                {
                    Limit = 10,
                    Sort = sort,
                });

                var viewModels = nextEvents.ToEnumerable().Select(e => new ListFutureEvents
                {
                    EventDescription = e.Description,
                    EventFinishesUTC = e.EndTime,
                    EventSlug = e.Id.ToString(),
                    EventStartsUTC = e.StartTime,
                    EventTitle = e.Title,
                    GroupName = group.Name,
                });

                result.AddRange(viewModels);
            }

            return result;
        }

        public async Task<Models.Event> RetrieveEvent(string eventID)
        {
            var events = EventsCollection();
            var group = await events.Find(u => u.Id == ObjectId.Parse(eventID)).SingleOrDefaultAsync();

            return group;
        }

        internal async Task InsertUser(User newUser)
        {
            var users = UsersCollection();

            await users.InsertOneAsync(newUser);
        }

        internal async Task<Models.User> GetUserByEmailAddress(string emailAddress)
        {
            var users = UsersCollection();

            var existingUser = await users.Find(u => u.EmailAddress == emailAddress).SingleOrDefaultAsync();

            return existingUser;
        }

        internal async Task<Models.User> GetUserByID(string uniqueId)
        {
            var users = UsersCollection();

            var filter = Builders<Models.User>.Filter.AnyEq(x => x.Identities, uniqueId);
            var existingUser = await users.Find(filter).SingleOrDefaultAsync();

            return existingUser;
        }

        private IMongoDatabase ConnectToDatabase()
        {
            var client = new MongoClient(_mongoDBSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDBSettings.DatabaseName);

            return database;
        }

        internal async Task UpdateEvent(Models.Event amendedEvent)
        {
            var events = EventsCollection();

            await events.ReplaceOneAsync(u => u.Id == amendedEvent.Id, amendedEvent);
        }

        internal async Task UpdateUser(User amendedUser)
        {
            var users = UsersCollection();

            await users.ReplaceOneAsync(u => u.Id == amendedUser.Id, amendedUser);
        }

        internal async Task UpdateGroup(Group group)
        {
            var groups = GroupsCollection();

            await groups.ReplaceOneAsync(grp => grp.Id == group.Id, group);
        }
    }
}
