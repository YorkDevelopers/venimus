using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VenimusAPIs.Models;
using VenimusAPIs.Mongo;

namespace CreateInitialData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Please wait...");

            var s = new VenimusAPIs.Settings.MongoDBSettings();
            s.ConnectionString = "mongodb+srv://app:ZsZoyFiGaWWxx6kG@cluster0-mwrwp.azure.mongodb.net/test?retryWrites=true&w=majority";
            s.DatabaseName = "venimus";

            var m = new MongoConnection(Options.Create(s));
            await m.ResetDatabase();

            var groups = m.GroupsCollection();
            var events = m.EventsCollection();


            foreach (var meetupFilename in Directory.GetFiles("Meetups", "*.markdown"))
            {
                Console.WriteLine($"{meetupFilename}"); 
                var fileContents = File.ReadAllLines(meetupFilename);
                var title = fileContents.First(line => line.StartsWith("title:")).Substring(7).Trim() ;
                var slug = title.ToUpper().Replace(" ", "").Replace("-", "");
                var imageName = fileContents.First(line => line.StartsWith("img:")).Substring("img: img/meetups/".Length).Trim();
                var strapLine = fileContents[8];
                var description = string.Join(System.Environment.NewLine, fileContents.Skip(8));

                var newGroup = new Group
                {
                    Name = title,
                    Slug = slug,
                    // StrapLine= strapLine,
                    Description = description,
                    Logo = await File.ReadAllBytesAsync("images/" + imageName),
                    IsActive = true,
                    SlackChannelName = "yorkcodedoj",
                    Members = new System.Collections.Generic.List<Group.GroupMember>(),
                };

                await groups.InsertOneAsync(newGroup);
            }

            //var evt = new Event
            //{
            //    Description = "This month will work together on some Advent of Code problems",
            //    GroupName = yorkDevelopers.Name,
            //    GroupId = yorkDevelopers.Id,
            //    Location = "York St John",
            //    GroupSlug = yorkDevelopers.Slug,
            //    Members = new System.Collections.Generic.List<Event.EventAttendees>(),
            //    Title = "December Meeting",
            //    StartTimeUTC = new DateTime(2019, 12, 11, 18, 30, 0),
            //    EndTimeUTC = new DateTime(2019, 12, 11, 21, 00, 0),
            //    GuestsAllowed = true,
            //    MaximumNumberOfAttendees = 26,
            //    Slug = "DEC2019",
            //};
            //await events.InsertOneAsync(evt);

            //evt = new Event
            //{
            //    Description = "This month will work together on some TDD problems",
            //    GroupName = yorkDevelopers.Name,
            //    GroupId = yorkDevelopers.Id,
            //    Location = "TBA",
            //    GroupSlug = yorkDevelopers.Slug,
            //    Members = new System.Collections.Generic.List<Event.EventAttendees>(),
            //    Title = "Jan Meeting",
            //    StartTimeUTC = new DateTime(2020, 1, 8, 18, 30, 0),
            //    EndTimeUTC = new DateTime(2020, 1, 8, 21, 00, 0),
            //    GuestsAllowed = true,
            //    MaximumNumberOfAttendees = 26,
            //    Slug = "JAN2020",
            //};
            //await events.InsertOneAsync(evt);

            Console.WriteLine("Done");
        }
    }
}
