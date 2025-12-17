using System.Net;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using Cff.Models;
using Cff.Error.Exceptions;
using CFFFusions.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CFFFusions.Services;

public class CodeforcesClient : ICodeforcesClient
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheExpirationTime = TimeSpan.FromMinutes(30);

    public CodeforcesClient(IMemoryCache cache, HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://codeforces.com/api/");
        _http.Timeout = TimeSpan.FromSeconds(20);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("CFFFusions/1.0");
        _cache = cache;
    }

    private static string E(string s) => UrlEncoder.Default.Encode(s);

    // ======================= USER =======================
    public async Task<CfUser> GetUserAsync(string handle, string lang = "en")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new CffError(
                    new BaseResponse(CffError.BAD_REQUEST, "Handle is required")
                );
            }

            var cacheKey = $"cf_user_{handle}_{lang}";
            if (_cache.TryGetValue<CfUser>(cacheKey, out var cachedUser))
            {
                return cachedUser;
            }

            var env = await GetEnvelopeAsync<List<CfUser>>(
                $"user.info?handles={E(handle)}&lang={E(lang)}"
            );

            var user = env.Result?.FirstOrDefault();
            if (user == null)
            {
                throw new CffError(
                    new BaseResponse(CffError.USER_NOT_FOUND, "User not found")
                );
            }

            _cache.Set(cacheKey, user, CacheExpirationTime);
            return user;
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
                    "Unexpected error while fetching user"
                ),
                ex: ex
            );
        }
    }

    // ======================= USER RATING =======================
    public async Task<List<CfRatingChange>> GetUserRatingAsync(string handle)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new CffError(
                    new BaseResponse(CffError.BAD_REQUEST, "Handle is required")
                );
            }

            var env = await GetEnvelopeAsync<List<CfRatingChange>>(
                $"user.rating?handle={E(handle)}"
            );

            return env.Result ?? new List<CfRatingChange>();
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
                    "Unexpected error while fetching rating history"
                ),
                ex: ex
            );
        }
    }

    // ======================= USER SUBMISSIONS =======================
    public async Task<List<CfSubmission>> GetUserSubmissionsAsync(
        string handle,
        int from = 1,
        int count = 100)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new CffError(
                    new BaseResponse(CffError.BAD_REQUEST, "Handle is required")
                );
            }

            count = Math.Clamp(count, 1, 1000);

            var env = await GetEnvelopeAsync<List<CfSubmission>>(
                $"user.status?handle={E(handle)}&from={from}&count={count}"
            );

            return env.Result ?? new List<CfSubmission>();
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
                    "Unexpected error while fetching submissions"
                ),
                ex: ex
            );
        }
    }

    // ======================= MULTIPLE USERS =======================
    public async Task<List<CfUser>> GetUsersAsync(string handlesCsv, string lang = "en")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(handlesCsv))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "At least one handle is required"
                    )
                );
            }

            var env = await GetEnvelopeAsync<List<CfUser>>(
                $"user.info?handles={E(handlesCsv)}&lang={E(lang)}"
            );

            return env.Result ?? new List<CfUser>();
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
                    "Unexpected error while fetching users"
                ),
                ex: ex
            );
        }
    }

    // ======================= ENVELOPE HANDLER =======================
    private async Task<CfEnvelope<T>> GetEnvelopeAsync<T>(string relativeUrl)
    {
        try
        {
            using var resp = await _http.GetAsync(relativeUrl);

            if (!resp.IsSuccessStatusCode)
            {
                // Rate limit
                if (resp.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new CffError(
                        new BaseResponse(
                            CffError.CODEFORCES_API_FAILED,
                            "Codeforces rate limit exceeded"
                        )
                    );
                }

                // 👉 YOUR CHOSEN RULE:
                // CF HTTP 400 == User not found
                if (resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new CffError(
                        new BaseResponse(
                            CffError.USER_NOT_FOUND,
                            "User not found on Codeforces"
                        )
                    );
                }

                // All other upstream failures
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

            // API-level failure
            if (!string.Equals(env.Status, "OK", StringComparison.OrdinalIgnoreCase))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.USER_NOT_FOUND,
                        env.Comment ?? "Codeforces API error"
                    )
                );
            }

            return env;
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
                    "Unexpected error while calling Codeforces"
                ),
                ex: ex
            );
        }
    }
}
