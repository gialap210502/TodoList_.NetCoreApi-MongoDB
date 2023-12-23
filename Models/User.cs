using MongoDB.Bson;

namespace TodoList.Models;

public class User
{
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    // Other user-related fields
}
