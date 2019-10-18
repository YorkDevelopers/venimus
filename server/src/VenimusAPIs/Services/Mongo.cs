using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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
            var collection = database.GetCollection<Models.Group>("groups");

            await collection.InsertOneAsync(group);
        }

        private IMongoDatabase ConnectToDatabase()
        {
            var client = new MongoClient(_mongoDBSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDBSettings.DatabaseName);

            return database;
        }
    }
}
