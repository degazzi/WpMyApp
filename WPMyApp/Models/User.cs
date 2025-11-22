// Models/User.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}";
}