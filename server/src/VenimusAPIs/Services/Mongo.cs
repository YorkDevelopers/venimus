﻿using System;
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

        private IMongoDatabase ConnectToDatabase()
        {
            var client = new MongoClient(_mongoDBSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDBSettings.DatabaseName);

            return database;
        }
    }
}
