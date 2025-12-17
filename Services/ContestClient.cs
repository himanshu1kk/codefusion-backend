using System.Net;
using System.Net.Http.Json;
using Cff.Models;
using Cff.Error.Exceptions;
using CFFFusions.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CFFFusions.Services;

public class ContestClient : IContestClient
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheExpirationTime = TimeSpan.FromMinutes(30);

    public ContestClient(IMemoryCache cache, HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://codeforces.com/api/");
        _http.Timeout = TimeSpan.FromSeconds(20);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("CFFFusions/1.0");
        _cache = cache;
    }

    // ---------------- CONTEST LIST ----------------
    public async Task<List<Contest>> GetAllContestsAsync(bool gym = false)
    {
        try
        {
            var env = await GetEnvelopeAsync<List<Contest>>($"contest.list?gym={gym}");
            return env.Result ?? new List<Contest>();
        }
        catch (CffError) { throw; }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error fetching contest list"
                ),
                ex: ex
            );
        }
    }

    // ---------------- RATING CHANGES ----------------
    public async Task<List<RatingChange>> GetContestRatingChangesAsync(int contestId)
    {
        try
        {
            var env = await GetEnvelopeAsync<List<RatingChange>>(
                $"contest.ratingChanges?contestId={contestId}"
            );
            return env.Result ?? new List<RatingChange>();
        }
        catch (CffError) { throw; }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error fetching contest rating changes"
                ),
                ex: ex
            );
        }
    }

    // ---------------- STANDINGS ----------------
    public async Task<ContestStandings> GetContestStandingsAsync(
        int contestId,
        int from = 1,
        int count = 10)
    {
        try
        {
            var env = await GetEnvelopeAsync<ContestStandings>(
                $"contest.standings?contestId={contestId}&from={from}&count={count}"
            );
            return env.Result!;
        }
        catch (CffError) { throw; }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error fetching contest standings"
                ),
                ex: ex
            );
        }
    }

    // ---------------- SUBMISSIONS ----------------
    public async Task<List<Submission>> GetContestSubmissionsAsync(
        int contestId,
        int from = 1,
        int count = 10)
    {
        try
        {
            var env = await GetEnvelopeAsync<List<Submission>>(
                $"contest.status?contestId={contestId}&from={from}&count={count}"
            );
            return env.Result ?? new List<Submission>();
        }
        catch (CffError) { throw; }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error fetching contest submissions"
                ),
                ex: ex
            );
        }
    }

    // ---------------- HACKS ----------------
    public async Task<List<Hack>> GetContestHacksAsync(int contestId, bool asManager = false)
    {
        try
        {
            var env = await GetEnvelopeAsync<List<Hack>>(
                $"contest.hacks?contestId={contestId}&asManager={asManager}"
            );
            return env.Result ?? new List<Hack>();
        }
        catch (CffError) { throw; }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error fetching contest hacks"
                ),
                ex: ex
            );
        }
    }

    // ---------------- ENVELOPE HANDLER ----------------
    private async Task<CfEnvelope<T>> GetEnvelopeAsync<T>(string relativeUrl)
    {
        try
        {
            using var resp = await _http.GetAsync(relativeUrl);

            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new CffError(
                        new BaseResponse(
                            CffError.CODEFORCES_API_FAILED,
                            "Codeforces rate limit exceeded"
                        )
                    );
                }

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
