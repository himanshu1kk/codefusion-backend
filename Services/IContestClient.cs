using System.Collections.Generic;
using System.Threading.Tasks;
using CFFFusions.Models;

namespace CFFFusions.Services
{
    public interface IContestClient
    {
        Task<List<Contest>> GetAllContestsAsync(bool gym = false);
        Task<List<RatingChange>> GetContestRatingChangesAsync(int contestId);
        Task<ContestStandings> GetContestStandingsAsync(int contestId, int from = 1, int count = 10);
        Task<List<Submission>> GetContestSubmissionsAsync(int contestId, int from = 1, int count = 10);
        Task<List<Hack>> GetContestHacksAsync(int contestId, bool asManager = false);
    }
}
