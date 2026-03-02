using CFFFusions.Models;

namespace CFFFusions.Services;

public interface INoteRepository
{
    Task UpsertAsync(Note note);
    Task<List<Note>> GetUserNotesAsync(string userId);
    Task<Note?> GetNoteAsync(string userId, int contestId, string index);
}