using TodoList.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TodoList.Services;

public class MongoDBService
{
    private readonly IMongoCollection<User> _userCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _userCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionName);
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
}