using Cff.Error.Exceptions;
using Cff.Models;
using CFFFusions.Models;

namespace CFFFusions.Services;

public class ProblemMetaService : IProblemMetaService
{
    private readonly ICodeforcesClient _cf;

    public ProblemMetaService(ICodeforcesClient cf)
    {
        _cf = cf;
    }

    public async Task<ProblemMetaDto> GetProblemMetaAsync(int contestId, string index)
    {
        try
        {
            if (contestId <= 0 || string.IsNullOrWhiteSpace(index))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "contestId and index are required"
                    )
                );
            }

            var problemSet = await _cf.GetProblemSetAsync();

            var problem = problemSet.Problems.FirstOrDefault(p =>
                p.ContestId == contestId &&
                string.Equals(p.Index, index, StringComparison.OrdinalIgnoreCase)
            );

            if (problem == null)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.INTERNAL_ERROR,
                        "Problem not found"
                    )
                );
            }

            return new ProblemMetaDto
            {
                ContestId = contestId,
                Index = problem.Index,
                Name = problem.Name,
                Rating = problem.Rating,
                Tags = problem.Tags ?? []
            };
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
                    "Unexpected error while fetching problem metadata"
                ),
                ex: ex
            );
        }
    }
}
