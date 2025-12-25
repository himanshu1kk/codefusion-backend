using System.Net;
using System.Net.Http.Json;
using Cff.Error.Exceptions;
using Cff.Models;
using CFFFusions.Models;

namespace CFFFusions.Services;

public class ProblemClient : IProblemClient
{
    private readonly HttpClient _http;

    public ProblemClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://codeforces.com/api/");
        _http.Timeout = TimeSpan.FromSeconds(20);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("CFFFusions/1.0");
    }

    public async Task<PagedResult<Problem>> GetProblemsAsync(
     string tag = "dp",
            int? minRating = null,
            int? maxRating = null,
            int page = 1,
            int pageSize = 20)
{
    try
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new CffError(
                new BaseResponse(CffError.BAD_REQUEST, "Tag is required")
            );
        }

        if (minRating.HasValue && maxRating.HasValue && minRating > maxRating)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.BAD_REQUEST,
                    "minRating cannot be greater than maxRating"
                )
            );
        }

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var env = await GetEnvelopeAsync<ProblemsetResponse>(
            $"problemset.problems?tags={Uri.EscapeDataString(tag)}"
        );

        var problems = env.Result?.Problems ?? new List<Problem>();

        // 🔹 Rating filter
        problems = problems
            .Where(p =>
                p.Rating.HasValue &&
                (!minRating.HasValue || p.Rating >= minRating) &&
                (!maxRating.HasValue || p.Rating <= maxRating)
            )
            .OrderBy(p => p.Rating)
            .ToList();

        var totalCount = problems.Count;

        var items = problems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Problem>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
    catch (CffError) { throw; }
    catch (Exception ex)
    {
        throw new CffError(
            new BaseResponse(
                CffError.INTERNAL_ERROR,
                "Error fetching problems"
            ),
            ex: ex
        );
    }
}



    private async Task<CfEnvelope<T>> GetEnvelopeAsync<T>(string relativeUrl)
    {
        try
        {
            using var resp = await _http.GetAsync(relativeUrl);

            if (!resp.IsSuccessStatusCode)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.CODEFORCES_API_FAILED,
                        $"Codeforces HTTP error {(int)resp.StatusCode}"
                    )
                );
            }

            var env = await resp.Content.ReadFromJsonAsync<CfEnvelope<T>>();

            if (env == null)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.CODEFORCES_API_FAILED,
                        "Empty response from Codeforces"
                    )
                );
            }

            if (!string.Equals(env.Status, "OK", StringComparison.OrdinalIgnoreCase))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.CODEFORCES_API_FAILED,
                        env.Comment ?? "Codeforces API error"
                    )
                );
            }

            return env;
        }
        catch (CffError) { throw; }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Unexpected error while calling Codeforces"
                ),
                ex: ex
            );
        }
    }
}
