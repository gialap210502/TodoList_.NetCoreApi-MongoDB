using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoList.Models;

public class TodoItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public string UserId { get; set; }
}