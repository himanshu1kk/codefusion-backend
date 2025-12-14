using System.Net;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using CFFFusions.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CFFFusions.Services;

public class CodeforcesClient : ICodeforcesClient
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheExpirationTime = TimeSpan.FromMinutes(30); // Example cache expiration time


    public CodeforcesClient(IMemoryCache cache , HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://codeforces.com/api/");
        _http.Timeout = TimeSpan.FromSeconds(20);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("CFFFusions/1.0");
         _cache = cache;
    }

    private static string E(string s) => UrlEncoder.Default.Encode(s);

      /// <summary>
    /// Method to get user details from cache first, then from external service if not found.
    /// </summary>
    private async Task<CfUser?> GetUserDetailsFromCacheAsync(string cacheKey)
    {
        try
        {
            // Check if the user details are in cache
            if (_cache.TryGetValue<CfUser>(cacheKey, out var cachedUser))
            {
                // Log and return the cached user
                return cachedUser;
            }
            return null; // User not found in cache
        }
        catch (Exception ex)
        {
            // Handle errors related to cache retrieval
            throw new InvalidOperationException($"Error retrieving user details from cache: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to get user details for a single handle.
    /// </summary>
    public async Task<CfUser> GetUserAsync(string handle, string lang = "en")
    {
        try
        {
            // Construct the cache key using the handle and language
            var cacheKey = $"CodeforcesUser_{handle}_{lang}";

            // Attempt to retrieve the user from the cache first
            var cachedUser = await GetUserDetailsFromCacheAsync(cacheKey);

            if (cachedUser != null)
            {
                // If user is found in cache, return the cached user
                return cachedUser;
            }

            // If not found in cache, fetch from the external service (Codeforces API)
            var env = await GetEnvelopeAsync<List<CfUser>>(
                $"user.info?handles={E(handle)}&lang={E(lang)}");

            var user = env.Result?.FirstOrDefault(); // Assume first user for single handle

            if (user == null)
            {
                // If no user found, throw an exception
                throw new InvalidOperationException("User not found");
            }

            // Log the user data (optional for debugging)
            Console.WriteLine(JsonSerializer.Serialize(user));

            // Store the fetched user data in the cache for future use
            _cache.Set(cacheKey, user, CacheExpirationTime); // Cache the user details

            // Return the user fetched from the external service
            return user;
        }
        catch (InvalidOperationException)
        {
            throw; // Rethrow the InvalidOperationException
        }
        catch (TaskCanceledException)
        {
            // Timeout error
            throw new InvalidOperationException("Request to Codeforces timed out.");
        }
        catch (HttpRequestException ex)
        {
            // Network error
            throw new InvalidOperationException($"Network error calling Codeforces: {ex.Message}");
        }
        catch (JsonException)
        {
            // Unexpected response format from Codeforces
            throw new InvalidOperationException("Unexpected response format from Codeforces.");
        }
        catch (Exception ex)
        {
            // General unknown error
            throw new InvalidOperationException($"Unknown error getting user '{handle}': {ex.Message}");
        }
    }


    public async Task<List<CfUser>> GetUsersAsync(string handlesCsv, string lang = "en")
    {
        try
        {
            var env = await GetEnvelopeAsync<List<CfUser>>(
                $"user.info?handles={E(handlesCsv)}&lang={E(lang)}");

            return env.Result ?? new List<CfUser>();
        }
        catch (InvalidOperationException) { throw; }
        catch (TaskCanceledException)
        {
            throw new InvalidOperationException("Request to Codeforces timed out.");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Network error calling Codeforces: {ex.Message}");
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Unexpected response format from Codeforces.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unknown error getting users '{handlesCsv}': {ex.Message}");
        }
    }

    public async Task<List<CfRatingChange>> GetUserRatingAsync(string handle)
    {
        try
        {
            var env = await GetEnvelopeAsync<List<CfRatingChange>>(
                $"user.rating?handle={E(handle)}");

            return env.Result ?? new List<CfRatingChange>();
        }
        catch (InvalidOperationException) { throw; }
        catch (TaskCanceledException)
        {
            throw new InvalidOperationException("Request to Codeforces timed out.");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Network error calling Codeforces: {ex.Message}");
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Unexpected response format from Codeforces.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unknown error getting rating for '{handle}': {ex.Message}");
        }
    }

    public async Task<List<CfSubmission>> GetUserSubmissionsAsync(string handle, int from = 1, int count = 100)
    {
        try
        {
            count = Math.Clamp(count, 1, 1000);
            var env = await GetEnvelopeAsync<List<CfSubmission>>(
                $"user.status?handle={E(handle)}&from={from}&count={count}");

            return env.Result ?? new List<CfSubmission>();
        }
        catch (InvalidOperationException) { throw; }
        catch (TaskCanceledException)
        {
            throw new InvalidOperationException("Request to Codeforces timed out.");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Network error calling Codeforces: {ex.Message}");
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Unexpected response format from Codeforces.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unknown error getting submissions for '{handle}': {ex.Message}");
        }
    }

    // ------------------ Private helper with its own try/catch ------------------
    private async Task<CfEnvelope<T>> GetEnvelopeAsync<T>(string relativeUrl)
    {
        try
        {
            using var resp = await _http.GetAsync(relativeUrl); // this using is used to close the connection so if we get the correct result or if any error occurs the conncection.dispose() is automatically called by this dispose method in itself..

            if (!resp.IsSuccessStatusCode) // line checks the HTTP-level response from the Codeforces API.
           // resp.IsSuccessStatusCode is a .NET property that checks if the status code is in the 2xx success range, i.e.:
           

                //200	OK
                //201	Created
                //202	Accepted
                //204	No Content
           
            {
                Console.WriteLine("did u came here");
                if (resp.StatusCode == (HttpStatusCode)429)
                    throw new InvalidOperationException("Codeforces rate limit hit. Try again later.");

                var body = await SafeReadBody(resp);
                  Console.WriteLine(JsonSerializer.Serialize(body));
                throw new HttpRequestException(
                    $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
            }
             Console.WriteLine("did u came here2");

            var env = await resp.Content.ReadFromJsonAsync<CfEnvelope<T>>();
            if (env is null)
                throw new InvalidOperationException("Empty response from Codeforces.");

            if (!string.Equals(env.Status, "OK", StringComparison.OrdinalIgnoreCase))
            {
                 Console.WriteLine("did u came here3");
                var reason = string.IsNullOrWhiteSpace(env.Comment) ? "Unknown error" : env.Comment!;
                throw new InvalidOperationException($"Codeforces API error: {reason}");
            }
            return env;
        }
        catch (TaskCanceledException) { throw; }     
        catch (HttpRequestException) { throw; }      
        catch (JsonException) { throw; }       
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
