using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VenimusAPIs.Models;
using Xunit;

namespace VenimusAPIs.Tests.Infrastucture
{
    [Collection("Serial")]
    public abstract class BaseTest : IClassFixture<Fixture>
    {
        public static class Cultures
        {
            public const string Normal = "en-GB";
            public const string Test = "zu-ZA";
        }

        protected string UniqueID { get; set; }

        protected string Token { get; set; }

        protected User User { get; set; }

        protected Fixture Fixture { get; }

        protected HttpResponseMessage Response { get; set; }

        protected Data Data { get; }

        public BaseTest(Fixture fixture)
        {
            Fixture = fixture;
            Data = new Data();
        }

        protected async Task WaitForServiceBus()
        {
            await Task.Delay(TimeSpan.FromSeconds(.5));
        }

        protected MongoDB.Driver.IMongoCollection<Models.GroupEvent> EventsCollection()
        {
            var mongoDatabase = Fixture.MongoDatabase();
            var collection = mongoDatabase.GetCollection<Models.GroupEvent>("events");

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

        protected async Task ResetDatabase()
        {
            var mongoDatabase = Fixture.MongoDatabase();
            await mongoDatabase.DropCollectionAsync("events");
            await mongoDatabase.DropCollectionAsync("groups");
            await mongoDatabase.DropCollectionAsync("users");
        }

        protected DateTime TrimMilliseconds(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
        }

        protected void AssertDateTime(DateTime dateTime1, DateTime dateTime2)
        {
            Assert.Equal(TrimMilliseconds(dateTime1), TrimMilliseconds(dateTime2));
        }

        protected async Task AssertBadRequest(string fieldName, string expectedErrorMessage)
        {
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, Response.StatusCode);

            var json = await Response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { IgnoreReadOnlyProperties = true };
            var validationProblemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(json, options);
            Assert.Equal(expectedErrorMessage, validationProblemDetails.Errors[fieldName].GetValue(0));
        }

        protected async Task AssertBadRequestDetail(string expectedErrorMessage)
        {
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, Response.StatusCode);

            var json = await Response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { IgnoreReadOnlyProperties = true };
            var validationProblemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(json, options);
            Assert.Equal(expectedErrorMessage, validationProblemDetails.Detail);
        }

        public async Task IAmASystemAdministrator()
        {
            UniqueID = Guid.NewGuid().ToString();
            Token = await Fixture.GetTokenForSystemAdministrator(UniqueID);

            User = Data.Create<Models.User>();
            User.Identities = new List<string> { UniqueID };

            var collection = UsersCollection();

            await collection.InsertOneAsync(User);

            Fixture.APIClient.SetBearerToken(Token);
        }

        public async Task IAmANormalUser()
        {
            UniqueID = Guid.NewGuid().ToString();
            Token = await Fixture.GetTokenForNormalUser(UniqueID);

            User = Data.Create<Models.User>();
            User.Identities = new List<string> { UniqueID };
            User.ProfilePicture = System.IO.File.ReadAllBytes("images/profile.jpg");

            var collection = UsersCollection();

            await collection.InsertOneAsync(User);

            Fixture.APIClient.SetBearerToken(Token);
        }
    }
}
