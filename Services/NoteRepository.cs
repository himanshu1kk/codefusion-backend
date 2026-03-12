using System.Text.Json;
using CFFFusions.Models;
using MongoDB.Driver;

namespace CFFFusions.Services;

public class NoteRepository : INoteRepository
{
    private readonly IMongoCollection<Note> _collection;

    public NoteRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Note>("notes");
    }

    public async Task UpsertAsync(Note note)
{
    var filter = Builders<Note>.Filter.Where(n =>
        n.UserId == note.UserId &&
        n.ContestId == note.ContestId &&
        n.Index == note.Index);

    var update = Builders<Note>.Update
        .Set(n => n.ProblemName, note.ProblemName)
        .Set(n => n.Tags, note.Tags)
        .Set(n => n.Rating, note.Rating)
        .Set(n => n.Notes, note.Notes)
        .Set(n => n.UpdatedAt, DateTime.UtcNow)
        .SetOnInsert(n => n.CreatedAt, DateTime.UtcNow);

    await _collection.UpdateOneAsync(
        filter,
        update,
        new UpdateOptions { IsUpsert = true });
}

    public async Task<List<Note>> GetUserNotesAsync(string userId)
    {
        return await _collection
            .Find(n => n.UserId == userId)
            .SortByDescending(n => n.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Note?> GetNoteAsync(string userId, int contestId, string index)
    {
        return await _collection
            .Find(n => n.UserId == userId &&
                       n.ContestId == contestId &&
                       n.Index == index)
            .FirstOrDefaultAsync();
    }
}