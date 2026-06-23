using CFFFusions.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CFFFusions.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IMongoCollection<RefreshToken> _refreshTokens;

    public RefreshTokenService(
        IMongoClient mongoClient,
        IOptions<MongoDbSettings> mongoOptions)
    {
        var database = mongoClient.GetDatabase(
            mongoOptions.Value.DatabaseName);

        _refreshTokens =
            database.GetCollection<RefreshToken>(
                "refreshTokens");
    }

    public async Task SaveAsync(
        RefreshToken refreshToken)
    {
        await _refreshTokens.InsertOneAsync(
            refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(
        string token)
    {
        return await _refreshTokens
            .Find(x => x.Token == token)
            .FirstOrDefaultAsync();
    }

    

    public async Task UpdateAsync(
        RefreshToken refreshToken)
    {
        await _refreshTokens.ReplaceOneAsync(
            x => x.Id == refreshToken.Id,
            refreshToken);
    }

    // revoke every token in the same family
    public async Task RevokeAllByFamilyIdAsync(
        string familyId)
    {
        var update = Builders<RefreshToken>.Update
            .Set(x => x.IsRevoked, true);

        await _refreshTokens.UpdateManyAsync(
            x => x.FamilyId == familyId,
            update);
    }
}