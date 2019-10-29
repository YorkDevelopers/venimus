using System.Collections.Generic;
using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class User
    {
        public ObjectId Id { get; set; }

        public string EmailAddress { get; set; }

        public List<string> Identities { get; set; }
    }
}
