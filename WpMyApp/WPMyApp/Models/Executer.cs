using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WpMyApp.Models
{
    public class Executer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
    }
}
