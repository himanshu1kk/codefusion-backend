using CFFFusions.Models;

namespace CFFFusions.Services;

public interface ICfAnalyticsService
{
    Task<List<CfEnrichedSubmission>> GetEnrichedSubmissionsAsync(
        string handle,
        int limit);

    Task<SkillGapResponse> GetSkillGapAsync(
        string handle,
        int limit);
}

