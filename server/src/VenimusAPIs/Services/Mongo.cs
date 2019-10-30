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

        public async Task StoreGroup(Models.Group group)
        {
            var database = ConnectToDatabase();
            var groups = database.GetCollection<Models.Group>("groups");

            await groups.InsertOneAsync(group);
        }

        public async Task<Models.Group> RetrieveGroup(string groupName)
        {
            var database = ConnectToDatabase();
            var groups = database.GetCollection<Models.Group>("groups");
            var group = await groups.Find(u => u.Name == groupName).SingleOrDefaultAsync();

            return group;
        }

        internal async Task StoreEvent(Event newEvent)
        {
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.Event>("events");

            await events.InsertOneAsync(newEvent);
        }

        internal async Task<List<ViewModels.FutureEvent>> GetFutureEvents()
        {
            var database = ConnectToDatabase();

            var events = database.GetCollection<Models.Event>("events");
            var groups = database.GetCollection<Models.Group>("groups");

            var currentTime = DateTime.UtcNow;
            
            var allGroups = await groups.FindAsync(Builders<Group>.Filter.Empty);
            var groupsList = await allGroups.ToListAsync();

            var result = new List<ViewModels.FutureEvent>();
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

                var viewModels = nextEvents.ToEnumerable().Select(e => new FutureEvent
                {
                    EventDescription = e.Description,
                    EventFinishesUTC = e.EndTime,
                    EventId = e.Id.ToString(),
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
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.Event>("events");
            var group = await events.Find(u => u.Id == ObjectId.Parse(eventID)).SingleOrDefaultAsync();

            return group;
        }

        internal async Task InsertUser(User newUser)
        {
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.User>("users");

            await events.InsertOneAsync(newUser);
        }

        internal async Task<Models.User> GetUserByEmailAddress(string emailAddress)
        {
            var database = ConnectToDatabase();
            var users = database.GetCollection<Models.User>("users");

            var existingUser = await users.Find(u => u.EmailAddress == emailAddress).SingleOrDefaultAsync();

            return existingUser;
        }

        internal async Task<Models.User> GetUserByID(string uniqueId)
        {
            var database = ConnectToDatabase();
            var users = database.GetCollection<Models.User>("users");

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
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.Event>("events");

            await events.ReplaceOneAsync(u => u.Id == amendedEvent.Id, amendedEvent);
        }

        internal async Task UpdateUser(User amendedUser)
        {
            var database = ConnectToDatabase();
            var users = database.GetCollection<Models.User>("users");

            await users.ReplaceOneAsync(u => u.Id == amendedUser.Id, amendedUser);
        }
    }
}
