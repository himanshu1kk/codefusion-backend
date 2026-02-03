using CFFFusions.Models;

namespace CFFFusions.Services;

public interface IProblemMetaService
{
    Task<ProblemMetaDto> GetProblemMetaAsync(int contestId, string index);
}
