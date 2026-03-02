using System.Text.Json;
using Cff.Error.Exceptions;
using Cff.Models;
using CFFFusions.Models;
using MongoDB.Bson;

namespace CFFFusions.Services;

public class NotesService : INotesService
{
    private readonly INoteRepository _repo;
    private readonly IProblemMetaService _problemMeta;

    public NotesService(
        INoteRepository repo,
        IProblemMetaService problemMeta)
    {
        _repo = repo;
        _problemMeta = problemMeta;
    }

    public async Task SaveNoteAsync(string userId, CreateNoteDto dto)
    {
        try
        {
            // if (string.IsNullOrWhiteSpace(userId))
            //     throw new CffError(new BaseResponse(CffError.USER_NOT_FOUND, "Unauthorized"));

            var meta = await _problemMeta.GetProblemMetaAsync(dto.ContestId, dto.Index);

            var note = new Note
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                ContestId = dto.ContestId,
                Index = dto.Index,
                ProblemName = meta.Name,
                Tags = meta.Tags,
                Rating = meta.Rating,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            Console.WriteLine(JsonSerializer.Serialize(note));

            await _repo.UpsertAsync(note);
        }
        catch (CffError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Unexpected error while saving note"),
                ex: ex);
        }
    }

    public Task<List<Note>> GetUserNotesAsync(string userId)
        => _repo.GetUserNotesAsync(userId);

    public Task<Note?> GetNoteAsync(string userId, int contestId, string index)
        => _repo.GetNoteAsync(userId, contestId, index);
}