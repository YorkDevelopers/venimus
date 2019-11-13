using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;
using VenimusAPIs.Models;

namespace VenimusAPIs.Mongo
{
    public abstract class MongoBase
    {
        private readonly Settings.MongoDBSettings _mongoDBSettings;

        private IMongoDatabase _cachedDatabase;

        public MongoBase(IOptions<Settings.MongoDBSettings> mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings.Value;
        }

        public async Task ResetDatabase()
        {
            var mongoDatabase = ConnectToDatabase();
            await mongoDatabase.DropCollectionAsync("events");
            await mongoDatabase.DropCollectionAsync("groups");
            await mongoDatabase.DropCollectionAsync("users");
        }

        protected IMongoCollection<Group> GroupsCollection()
        {
            var database = ConnectToDatabase();
            var groups = database.GetCollection<Models.Group>("groups");
            return groups;
        }

        protected IMongoCollection<User> UsersCollection()
        {
            var database = ConnectToDatabase();
            var users = database.GetCollection<Models.User>("users");
            return users;
        }

        protected IMongoCollection<Event> EventsCollection()
        {
            var database = ConnectToDatabase();
            var events = database.GetCollection<Models.Event>("events");
            return events;
        }

        protected IMongoDatabase ConnectToDatabase()
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
