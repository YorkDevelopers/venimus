using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.Mongo;

namespace CreateInitialData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var s = new VenimusAPIs.Settings.MongoDBSettings();
            s.ConnectionString = "mongodb+srv://app:ZsZoyFiGaWWxx6kG@cluster0-mwrwp.azure.mongodb.net/test?retryWrites=true&w=majority";
            s.DatabaseName = "venimus";

            var m = new MongoConnection(Options.Create(s));
            await m.ResetDatabase();

            var groups = m.GroupsCollection();
            var events = m.EventsCollection();

            var yorkDevelopers = new Group
            {
                Name = "York Code Dojo",
                Slug = "YORKCODEDOJO",
                Description = "York Code Dojo is a group dedicated to programming. We focus on practical coding sessions , where you can learn by doing.",
                Logo = await File.ReadAllBytesAsync("images/York_Code_Dojo.jpg"),
                IsActive = true,
                SlackChannelName = "yorkcodedoj",
                Members = new System.Collections.Generic.List<Group.GroupMember>(),
            };

            await groups.InsertOneAsync(yorkDevelopers);

            var evt = new Event
            {
                Description = "This month will work together on some Advent of Code problems",
                GroupName = yorkDevelopers.Name,
                GroupId = yorkDevelopers.Id,
                Location = "York St John",
                GroupSlug = yorkDevelopers.Slug,
                Members = new System.Collections.Generic.List<Event.EventAttendees>(),
                Title = "December Meeting",
                StartTimeUTC = new DateTime(2019, 12, 11, 18, 30, 0),
                EndTimeUTC = new DateTime(2019, 12, 11, 21, 00, 0),
                GuestsAllowed = true,
                MaximumNumberOfAttendees = 26,
                Slug = "DEC2019",
            };
            await events.InsertOneAsync(evt);


            var codeAndCoffee = new Group
            {
                Name = "York Code and Coffee",
                Slug = "YORKCODEANDCOFFEE",
                Description = "York Code And Coffee - An informal meeting of Software Developers and Tech Professionals before we start our working day. We meet at in a coffee shop from 7:30am onwards twice a week.",
                Logo = await File.ReadAllBytesAsync("images/code_and_coffee_logo_small.bmp"),
                IsActive = true,
                SlackChannelName = "codeandcoffee",
                Members = new System.Collections.Generic.List<Group.GroupMember>(),
            };

            await groups.InsertOneAsync(codeAndCoffee);
        }
    }
}
