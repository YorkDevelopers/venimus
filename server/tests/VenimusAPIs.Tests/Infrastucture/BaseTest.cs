using Xunit;

namespace VenimusAPIs.Tests.Infrastucture
{
    public abstract class BaseTest : IClassFixture<Fixture>
    {
        protected Fixture Fixture { get; }

        protected Data Data { get; }

        public BaseTest(Fixture fixture)
        {
            Fixture = fixture;
            Data = new Data();
        }

        protected MongoDB.Driver.IMongoCollection<Models.Event> EventsCollection()
        {
            var mongoDatabase = Fixture.MongoDatabase();
            var collection = mongoDatabase.GetCollection<Models.Event>("events");
         
            return collection;
        }

        protected MongoDB.Driver.IMongoCollection<Models.Group> GroupsCollection()
        {
            var mongoDatabase = Fixture.MongoDatabase();
            var collection = mongoDatabase.GetCollection<Models.Group>("groups");

            return collection;
        }

        protected MongoDB.Driver.IMongoCollection<Models.User> UsersCollection()
        {
            var mongoDatabase = Fixture.MongoDatabase();
            var collection = mongoDatabase.GetCollection<Models.User>("users");

            return collection;
        }
    }
}
