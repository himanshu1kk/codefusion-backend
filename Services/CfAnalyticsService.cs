using Cff.Error.Exceptions;
using Cff.Models;
using CFFFusions.Models;

namespace CFFFusions.Services;

public class CfAnalyticsService : ICfAnalyticsService
{
    private readonly ICodeforcesClient _cf;

    public CfAnalyticsService(ICodeforcesClient cf)
    {
        _cf = cf;
    }

    public async Task<List<CfEnrichedSubmission>> GetEnrichedSubmissionsAsync(
        string handle,
        int limit)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "Handle is required"
                    )
                );
            }

            var submissions = await _cf.GetUserSubmissionsAsync(handle, 1, limit);
            var problemSet = await _cf.GetProblemSetAsync();

            var problemMap = new Dictionary<string, Problem>();

            foreach (var p in problemSet.Problems)
            {
                if (p.ContestId == null || string.IsNullOrWhiteSpace(p.Index))
                    continue;

                var key = $"{p.ContestId}{p.Index}";
                problemMap[key] = p;
            }

            var result = new List<CfEnrichedSubmission>();

            foreach (var sub in submissions)
            {
                if (sub.Problem?.ContestId == null || sub.Problem?.Index == null)
                    continue;

                var key = $"{sub.Problem.ContestId}{sub.Problem.Index}";

                if (!problemMap.TryGetValue(key, out var problem))
                    continue;

                result.Add(new CfEnrichedSubmission
                {
                    ContestId = sub.Problem.ContestId.Value,
                    Index = sub.Problem.Index,
                    Verdict = sub.Verdict,
                    Rating = problem.Rating,
                    Tags = problem.Tags ?? []
                });
            }

            return result;
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
                    "Unexpected error while building enriched submissions"
                ),
                ex: ex
            );
        }
    }

    public async Task<SkillGapResponse> GetSkillGapAsync(
        string handle,
        int limit)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "Handle is required"
                    )
                );
            }

            var enriched = await GetEnrichedSubmissionsAsync(handle, limit);
            var tagMap = new Dictionary<string, TagStats>();

            foreach (var sub in enriched)
            {
                foreach (var tag in sub.Tags)
                {
                    if (!tagMap.TryGetValue(tag, out var stats))
                    {
                        stats = new TagStats
                        {
                            Tag = tag
                        };
                        tagMap[tag] = stats;
                    }

                    stats.Attempts++;

                    if (sub.Verdict == "OK")
                        stats.Accepted++;
                    else if (sub.Verdict == "WRONG_ANSWER")
                        stats.WrongAnswer++;
                    else if (sub.Verdict == "TIME_LIMIT_EXCEEDED")
                        stats.TimeLimitExceeded++;
                }
            }

            var response = new SkillGapResponse();

            foreach (var stats in tagMap.Values)
            {
                if (stats.Attempts < 5)
                    continue;

                if (stats.Accuracy < 30)
                {
                    stats.Level = "Weak";
                    response.Weak.Add(stats);
                }
                else if (stats.Accuracy <= 60)
                {
                    stats.Level = "Average";
                    response.Average.Add(stats);
                }
                else
                {
                    stats.Level = "Strong";
                    response.Strong.Add(stats);
                }
            }

            return response;
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
                    "Unexpected error while computing skill gap analysis"
                ),
                ex: ex
            );
        }
    }
}
