using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VenimusAPIs.Models;

namespace VenimusAPIs.Mongo
{
    public class MongoConnection
    {
        private readonly Settings.MongoDBSettings _mongoDBSettings;

        private IMongoDatabase? _cachedDatabase;

        public MongoConnection(IOptions<Settings.MongoDBSettings> mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings.Value;
        }

        public IMongoCollection<Group> GroupsCollection()
        {
            var database = ConnectToDatabase();
            var groups = database.GetCollection<Models.Group>("groups");
            return groups;
        }

        public IMongoCollection<User> UsersCollection()
        {
            var database = ConnectToDatabase();
            var users = database.GetCollection<Models.User>("users");
            return users;
        }

        public IMongoCollection<GroupEvent> EventsCollection()
        {
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.GroupEvent>("events");
            return events;
        }

        public IMongoDatabase ConnectToDatabase()
        {
            if (_cachedDatabase == null)
            {
                var client = new MongoClient(_mongoDBSettings.ConnectionString);
                _cachedDatabase = client.GetDatabase(_mongoDBSettings.DatabaseName);
            }

            return _cachedDatabase;
        }
    }
}
