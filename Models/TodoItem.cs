using MongoDB.Bson;

namespace TodoList.Models;

public class TodoItem
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
}