﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using VenimusAPIs.Models;

namespace VenimusAPIs.Mongo
{
    public class UserStore
    {
        private readonly MongoConnection _mongoConnection;

        public UserStore(MongoConnection mongoConnection)
        {
            _mongoConnection = mongoConnection;
        }

        internal async Task InsertUser(User newUser)
        {
            var users = _mongoConnection.UsersCollection();

            await users.InsertOneAsync(newUser).ConfigureAwait(false);
        }

        internal async Task UpdateUser(User amendedUser)
        {
            var users = _mongoConnection.UsersCollection();

            await users.ReplaceOneAsync(u => u.Id == amendedUser.Id, amendedUser).ConfigureAwait(false);
        }

        internal async Task<Models.User> GetUserByEmailAddress(string emailAddress)
        {
            var users = _mongoConnection.UsersCollection();

            var existingUser = await users.Find(u => u.EmailAddress == emailAddress).SingleOrDefaultAsync().ConfigureAwait(false);

            return existingUser;
        }

        internal async Task<Models.User?> GetUserById(ObjectId id)
        {
            var users = _mongoConnection.UsersCollection();

            var existingUser = await users.Find(u => u.Id == id).SingleOrDefaultAsync().ConfigureAwait(false);

            return existingUser;
        }

        internal async Task<Models.User?> GetUserByApprovalId(Guid guid)
        {
            var users = _mongoConnection.UsersCollection();

            var existingUser = await users.Find(u => u.ApprovalID == guid).SingleOrDefaultAsync().ConfigureAwait(false);

            return existingUser;
        }

        internal async Task<Models.User?> GetUserByDisplayName(string displayName)
        {
            var users = _mongoConnection.UsersCollection();

            var existingUser = await users.Find(u => u.DisplayName == displayName).SingleOrDefaultAsync().ConfigureAwait(false);

            return existingUser;
        }

        internal async Task<Models.User> GetUserByID(string uniqueId)
        {
            var users = _mongoConnection.UsersCollection();

            var filter = Builders<Models.User>.Filter.AnyEq(x => x.Identities, uniqueId);
            var existingUser = await users.Find(filter).SingleOrDefaultAsync().ConfigureAwait(false);

            return existingUser;
        }
    }
}
