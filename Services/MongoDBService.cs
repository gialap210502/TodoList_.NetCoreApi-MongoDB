using TodoList.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace TodoList.Services;

public class MongoDBService
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<TodoItem> _todoCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _userCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionName);
        _todoCollection = database.GetCollection<TodoItem>(mongoDBSettings.Value.TodoCollection);
    }

    public async Task Register(User user)
    {
        await _userCollection.InsertOneAsync(user);
        return;
    }
    public async Task<List<User>> GetAsync()
    {
        return await _userCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<User> Login(string username, string password)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Username, username);
        var user = await _userCollection.Find(filter).SingleOrDefaultAsync();
        if (user == null)
        {
            // User with provided username doesn't exist
            return null;
        }
        // Validate the password here (You should handle password hashing and verification securely)
        if (user.Password == password)
        {
            // Password matches
            return user;
        }
        else
        {
            // Password doesn't match
            return null;
        }
    }

    //todo service

    //get
    public async Task<List<TodoItem>> GetTodoItemsAsync(string userId)
    {
        var filter = Builders<TodoItem>.Filter.Eq(item => item.UserId, userId);
        return await _todoCollection.Find(filter).ToListAsync();
    }

    //create
    public async Task Create(TodoItem todoItem)
    {
        await _todoCollection.InsertOneAsync(todoItem);
        return;
    }

    //edit 
    public async Task Update(string id, string name, string description, int status)
    {
        FilterDefinition<TodoItem> filter = Builders<TodoItem>.Filter.Eq("Id", id);
        var update = Builders<TodoItem>.Update
            .Set("Name", name)
            .Set("Description", description)
            .Set("Status", status);
        await _todoCollection.UpdateOneAsync(filter, update);
        return;
    }

    //delete
    public async Task Delete(string id)
    {
        FilterDefinition<TodoItem> filter = Builders<TodoItem>.Filter.Eq("Id", id);
        await _todoCollection.DeleteOneAsync(filter);
    }
}