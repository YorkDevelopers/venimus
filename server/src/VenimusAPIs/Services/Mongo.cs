using System;
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

        public async Task<Models.Event> RetrieveEvent(string eventID)
        {
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.Event>("events");
            var group = await events.Find(u => u._id == ObjectId.Parse(eventID)).SingleOrDefaultAsync();

            return group;
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

            await events.ReplaceOneAsync(u => u._id == amendedEvent._id, amendedEvent);
        }
    }
}
