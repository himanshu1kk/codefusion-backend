using CFFFusions.Models;
using System.Collections.Generic;

namespace CFFFusions.Services;

public interface ICodeforcesClient
{
    Task<CfUser> GetUserAsync(string handle, string lang = "en");
    Task<List<CfUser>> GetUsersAsync(string handlesCsv, string lang = "en");

    Task<List<CfRatingChange>> GetUserRatingAsync(string handle);
    Task<List<CfSubmission>> GetUserSubmissionsAsync(string handle, int from = 1, int count = 100);
      Task<ProblemSetResponses> GetProblemSetAsync();
}
