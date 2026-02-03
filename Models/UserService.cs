using CFFFusions.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CFFFusions.Services;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(
        IMongoClient mongoClient,
        IOptions<MongoDbSettings> mongoOptions)
    {
        var database = mongoClient.GetDatabase(mongoOptions.Value.DatabaseName);
        _users = database.GetCollection<User>("users");
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users
            .Find(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task SaveAsync(User user)
{
    await _users.ReplaceOneAsync(
        u => u.UserId == user.UserId,
        user
    );
}
}
