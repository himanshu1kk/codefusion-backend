using CFFFusions.Models;

namespace CFFFusions.Services;

public interface INotesService
{
    Task SaveNoteAsync(string userId, CreateNoteDto dto);
    Task<List<Note>> GetUserNotesAsync(string userId);
    Task<Note?> GetNoteAsync(string userId, int contestId, string index);
}