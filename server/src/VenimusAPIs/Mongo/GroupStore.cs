using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VenimusAPIs.Models;

namespace VenimusAPIs.Mongo
{
    public class GroupStore
    {
        private readonly MongoConnection _mongoConnection;

        public GroupStore(MongoConnection mongoConnection)
        {
            _mongoConnection = mongoConnection;
        }

        internal async Task<List<Models.Group>> GetActiveGroups()
        {
            var groups = _mongoConnection.GroupsCollection();
            var models = await groups.Find(u => u.IsActive).ToListAsync();

            return models;
        }

        internal async Task DeleteGroup(Group group)
        {
            var groups = _mongoConnection.GroupsCollection();

            await groups.DeleteOneAsync(grp => grp.Id == group.Id);
        }

        internal async Task<List<Group>> RetrieveMyActiveGroups(ObjectId userID)
        {
            var groups = _mongoConnection.GroupsCollection();

            var memberMatch = Builders<Group.GroupMember>.Filter.Eq(a => a.UserId, userID);

            var filter = Builders<Group>.Filter.ElemMatch(x => x.Members, memberMatch) &
                         Builders<Group>.Filter.Eq(ent => ent.IsActive, true);
            var matchingGroups = await groups.Find(filter).ToListAsync();

            return matchingGroups;
        }

        public async Task StoreGroup(Models.Group group)
        {
            var groups = _mongoConnection.GroupsCollection();

            await groups.InsertOneAsync(group);
        }

        public async Task<Models.Group?> RetrieveGroupBySlug(string groupSlug)
        {
            var groups = _mongoConnection.GroupsCollection();
            var group = await groups.Find(u => u.Slug == groupSlug).SingleOrDefaultAsync();

            if (group != null)
            {
                if (group.Members == null)
                {
                    group.Members = new List<Group.GroupMember>();
                }
            }

            return group;
        }

        public async Task<Models.Group?> RetrieveGroupByName(string groupName)
        {
            var groups = _mongoConnection.GroupsCollection();
            var group = await groups.Find(u => u.Name == groupName).SingleOrDefaultAsync();

            if (group != null)
            {
                if (group.Members == null)
                {
                    group.Members = new List<Group.GroupMember>();
                }
            }

            return group;
        }

        internal async Task UpdateGroup(Group group)
        {
            var groups = _mongoConnection.GroupsCollection();

            await groups.ReplaceOneAsync(grp => grp.Id == group.Id, group);
        }

        public async Task<Models.Group?> RetrieveGroupByGroupId(ObjectId groupId)
        {
            var groups = _mongoConnection.GroupsCollection();
            var group = await groups.Find(u => u.Id == groupId).SingleOrDefaultAsync();

            if (group != null)
            {
                if (group.Members == null)
                {
                    group.Members = new List<Group.GroupMember>();
                }
            }

            return group;
        }

        public async Task<List<Models.Group>> RetrieveAllGroups()
        {
            var groups = _mongoConnection.GroupsCollection();
            var models = await groups.Find(u => true).ToListAsync();

            return models;
        }

        internal async Task UpdateUserDetailsInGroups(User user)
        {
            var groups = _mongoConnection.GroupsCollection();

            var memberMatch = Builders<Group.GroupMember>.Filter.Eq(a => a.UserId, user.Id);
            var filter = Builders<Group>.Filter.ElemMatch(x => x.Members, memberMatch);
            var groupsTheUserBelongsTo = await groups.Find(filter).ToListAsync();

            foreach (var grp in groupsTheUserBelongsTo)
            {
                var member = grp.Members.Single(g => g.UserId == user.Id);
                member.Bio = user.Bio;
                member.DisplayName = user.DisplayName;
                member.EmailAddress = user.EmailAddress;
                member.Fullname = user.Fullname;
                member.Pronoun = user.Pronoun;
                await groups.ReplaceOneAsync(u => u.Id == grp.Id, grp);
            }
        }
    }
}
