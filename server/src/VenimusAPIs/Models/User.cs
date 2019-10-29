using MongoDB.Bson;

namespace VenimusAPIs.Models
{
    public class User
    {
        public ObjectId Id { get; set; }

        public string EmailAddress { get; set; }

        public string[] Identities { get; set; }
    }
}
