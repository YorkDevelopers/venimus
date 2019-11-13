using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.Settings;

namespace VenimusAPIs.Mongo
{
    public class UserStore : MongoBase
    {
        public UserStore(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
        }

        internal async Task InsertUser(User newUser)
        {
            var users = UsersCollection();

            await users.InsertOneAsync(newUser);
        }

        internal async Task UpdateUser(User amendedUser)
        {
            var users = UsersCollection();

            await users.ReplaceOneAsync(u => u.Id == amendedUser.Id, amendedUser);
        }

        internal async Task<Models.User> GetUserByEmailAddress(string emailAddress)
        {
            var users = UsersCollection();

            var existingUser = await users.Find(u => u.EmailAddress == emailAddress).SingleOrDefaultAsync();

            return existingUser;
        }

        internal async Task<Models.User> GetUserById(ObjectId id)
        {
            var users = UsersCollection();

            var existingUser = await users.Find(u => u.Id == id).SingleOrDefaultAsync();

            return existingUser;
        }

        internal async Task<Models.User> GetUserByDisplayName(string displayName)
        {
            var users = UsersCollection();

            var existingUser = await users.Find(u => u.DisplayName == displayName).SingleOrDefaultAsync();

            return existingUser;
        }

        internal async Task<Models.User> GetUserByID(string uniqueId)
        {
            var users = UsersCollection();

            var filter = Builders<Models.User>.Filter.AnyEq(x => x.Identities, uniqueId);
            var existingUser = await users.Find(filter).SingleOrDefaultAsync();

            return existingUser;
        }

        internal async Task<List<Models.User>> GetUsersByIds(IEnumerable<ObjectId> memberIds)
        {
            var users = UsersCollection();

            var filter = Builders<Models.User>.Filter.In(x => x.Id, memberIds);
            var matchingUsers = await users.Find(filter).ToListAsync();

            return matchingUsers;
        }
    }
}
