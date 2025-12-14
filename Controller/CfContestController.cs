using CFFFusions.Models;
using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CFFFusions.Controllers
{
    [ApiController]
    [Route("api/v0.1")]
    public class CfContestController : ControllerBase
    {
        private readonly IContestClient _contestClient;

        public CfContestController(IContestClient contestClient)
        {
            _contestClient = contestClient;
        }

        // Get all contests (e.g., gym or regular contests)

        //the response for this api is 

        /*	
Response body
Download
[
  {
    "id": 2178,
    "name": "Good Bye 2025",
    "type": "CF",
    "phase": "BEFORE",
    "frozen": false,
    "durationSeconds": 10800,
    "freezeDurationSeconds": null,
    "startTimeSeconds": 1766846100,
    "relativeTimeSeconds": -1114244,
    "preparedBy": null,
    "websiteUrl": null,
    "description": null,
    "difficulty": null,
    "kind": null,
    "icpcRegion": null,
    "country": null,
    "city": null,
    "season": null
  },
  {
    "id": 2177,
    "name": "ICPC 2025 Online Winter Challenge powered by Huawei",
    "type": "IOI",
    "phase": "BEFORE",
    "frozen": false,
    "durationSeconds": 1382400,
    "freezeDurationSeconds": null,
    "startTimeSeconds": 1766746800,
    "relativeTimeSeconds": -1014944,
    "preparedBy": null,
    "websiteUrl": null,
    "description": null,
    "difficulty": null,
    "kind": null,
    "icpcRegion": null,
    "country": null,
    "city": null,
    "season": null
  },
  {
    "id": 2179,
    "name": "Codeforces Round (Div. 3)",
    "type": "ICPC",
    "phase": "BEFORE",
    "frozen": false,
    "durationSeconds": 9000,
    "freezeDurationSeconds": null,
    "startTimeSeconds": 1766500500,
    "relativeTimeSeconds": -768644,
    "preparedBy": null,
    "websiteUrl": null,
    "description": null,
    "difficulty": null,
    "kind": null,
    "icpcRegion": null,
    "country": null,
    "city": null,
    "season": null
  },
  {
    "id": 2180,
    "name": "Codeforces Global Round 31 (Div. 1 + Div. 2)",
    "type": "CF",
    "phase": "BEFORE",
    "frozen": false,
    "durationSeconds": 9000,
    "freezeDurationSeconds": null,
    "startTimeSeconds": 1766154900,
    "relativeTimeSeconds": -423044,
    "preparedBy": null,
    "websiteUrl": null,
    "description": null,
    "difficulty": null,
    "kind": null,
    "icpcRegion": null,
    "country": null,
    "city": null,
    "season": null
  },*/
        [HttpGet("cf/contest")]

        
        public async Task<IActionResult> GetAllContests([FromQuery] bool gym = false)
        {
            try
            {
                var contests = await _contestClient.GetAllContestsAsync(gym);
                Console.WriteLine(JsonSerializer.Serialize(contests));
                return Ok(contests);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        // Get rating changes for a specific contest

        /*
        if the contestid is 10 then this is what we get in the response
        
        [
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "Petr",
    "rank": 1,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 2046,
    "newRating": 2109
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "Romka",
    "rank": 2,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1675,
    "newRating": 1852
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "andrewzta",
    "rank": 3,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1588,
    "newRating": 1790
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "ainu7",
    "rank": 4,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1669,
    "newRating": 1844
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "LemonTree",
    "rank": 5,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1740,
    "newRating": 1889
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "RoBa",
    "rank": 6,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1749,
    "newRating": 1892
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "it4.kp",
    "rank": 7,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1836,
    "newRating": 1946
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "Philip_PV",
    "rank": 8,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1753,
    "newRating": 1891
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "dasko1",
    "rank": 9,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1670,
    "newRating": 1835
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "gawry",
    "rank": 10,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1783,
    "newRating": 1905
  },
  {
    "contestId": 10,
    "contestName": "Codeforces Beta Round 10",
    "handle": "Rydberg",
    "rank": 11,
    "ratingUpdateTimeSeconds": 1271353500,
    "oldRating": 1675,
    "newRating": 1835
  },
  {*/
        [HttpGet("cf/contest/{contestId}/rating-changes")]
        public async Task<IActionResult> GetContestRatingChanges(int contestId)
        {
            try
            {
                var ratingChanges = await _contestClient.GetContestRatingChangesAsync(contestId);
                return Ok(ratingChanges);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        // Get contest standings
        [HttpGet("cf/contest/{contestId}/standings")]
        public async Task<IActionResult> GetContestStandings(int contestId, [FromQuery] int from = 1, [FromQuery] int count = 10)
        {
            try
            {
                var standings = await _contestClient.GetContestStandingsAsync(contestId, from, count);
                return Ok(standings);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        // Get submissions for a contest
        [HttpGet("cf/contest/{contestId}/submissions")]
        public async Task<IActionResult> GetContestSubmissions(int contestId, [FromQuery] int from = 1, [FromQuery] int count = 10)
        {
            try
            {
                var submissions = await _contestClient.GetContestSubmissionsAsync(contestId, from, count);
                return Ok(submissions);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        // Get hacks for a specific contest
        [HttpGet("cf/contest/{contestId}/hacks")]
        public async Task<IActionResult> GetContestHacks(int contestId, [FromQuery] bool asManager = false)
        {
            try
            {
                var hacks = await _contestClient.GetContestHacksAsync(contestId, asManager);
                return Ok(hacks);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
    }
}
