using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CFFFusions.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CFFFusions.Services
{
    public class ContestClient : IContestClient
    {
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheExpirationTime = TimeSpan.FromMinutes(30); // Example cache expiration time

        public ContestClient(IMemoryCache cache, HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://codeforces.com/api/");
            _http.Timeout = TimeSpan.FromSeconds(20);
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("CFFFusions/1.0");
            _cache = cache;
        }

        private static string E(string s) => System.Text.Encodings.Web.UrlEncoder.Default.Encode(s);

        // Method to get contests
        public async Task<List<Contest>> GetAllContestsAsync(bool gym = false)
        {
            var env = await GetEnvelopeAsync<List<Contest>>($"contest.list?gym={gym}");
            return env.Result ?? new List<Contest>();
        }

        // Method to get rating changes for a contest
        public async Task<List<RatingChange>> GetContestRatingChangesAsync(int contestId)
        {
            var env = await GetEnvelopeAsync<List<RatingChange>>($"contest.ratingChanges?contestId={contestId}");
            return env.Result ?? new List<RatingChange>();
        }

        // Method to get contest standings
        public async Task<ContestStandings> GetContestStandingsAsync(int contestId, int from = 1, int count = 10)
        {
            var env = await GetEnvelopeAsync<ContestStandings>($"contest.standings?contestId={contestId}&from={from}&count={count}");
            return env.Result;
        }

        // Method to get submissions for a contest
        public async Task<List<Submission>> GetContestSubmissionsAsync(int contestId, int from = 1, int count = 10)
        {
            var env = await GetEnvelopeAsync<List<Submission>>($"contest.status?contestId={contestId}&from={from}&count={count}");
            return env.Result ?? new List<Submission>();
        }

        // Method to get hacks for a contest
        public async Task<List<Hack>> GetContestHacksAsync(int contestId, bool asManager = false)
        {
            var env = await GetEnvelopeAsync<List<Hack>>($"contest.hacks?contestId={contestId}&asManager={asManager}");
            return env.Result ?? new List<Hack>();
        }

        // ------------------ Private helper with its own try/catch ------------------
        private async Task<CfEnvelope<T>> GetEnvelopeAsync<T>(string relativeUrl)
        {
            try
            {
                using var resp = await _http.GetAsync(relativeUrl);
                if (!resp.IsSuccessStatusCode)
                {
                    var body = await SafeReadBody(resp);
                    throw new HttpRequestException($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
                }

                var env = await resp.Content.ReadFromJsonAsync<CfEnvelope<T>>();
                if (env == null)
                    throw new InvalidOperationException("Empty response from Codeforces.");

                if (!string.Equals(env.Status, "OK", StringComparison.OrdinalIgnoreCase))
                {
                    var reason = string.IsNullOrWhiteSpace(env.Comment) ? "Unknown error" : env.Comment;
                    throw new InvalidOperationException($"Codeforces API error: {reason}");
                }
                return env;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error calling Codeforces: {ex.Message}");
            }
        }

        private static async Task<string> SafeReadBody(HttpResponseMessage resp)
        {
            try { return await resp.Content.ReadAsStringAsync(); }
            catch { return "<unreadable body>"; }
        }
    }
}
