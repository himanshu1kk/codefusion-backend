using CFFFusions.Models;

namespace CFFFusions.Services
{
    public interface IProblemClient
    {
        Task<PagedResult<Problem>> GetProblemsAsync(
            string tag = "dp",
            int? minRating = null,
            int? maxRating = null,
            int page = 1,
            int pageSize = 20
        );
    }
}
