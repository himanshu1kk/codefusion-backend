using CFFFusions.Models;
using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/v0.1")]
public class CfController(ICodeforcesClient cf) : ControllerBase
{
    private readonly ICodeforcesClient _cf = cf;


    /// <summary>
    /// 
    /// what this api does : - fetches the user details 
    /// 
    /// http://localhost:5243/api/v0.1/cf/user/orzdevinwang 
    /// orzdevinawang is the users handle and the response is below for this:
    /// {
    /*   "handle": "orzdevinwang",
       "email": null,
       "vkId": null,
       "openId": null,
       "firstName": "Kangyang",
       "lastName": "Zhou",
       "country": "China",
       "city": "Hangzhou",
       "organization": "Explosive Vegetable Club",
       "contribution": 30,
       "rank": "legendary grandmaster",
       "rating": 3670,
       "maxRank": "legendary grandmaster",
       "maxRating": 3844,
       "lastOnlineTimeSeconds": 1760404461,
       "registrationTimeSeconds": 1583069392,
       "friendOfCount": 7139,
       "avatar": "https://userpic.codeforces.org/no-avatar.jpg",
       "titlePhoto": "https://userpic.codeforces.org/no-title.jpg"
       }
       */
    /// 
    /// </summary>



    [HttpGet("cf/user/{handle}")]
    public async Task<IActionResult> GetUser(string handle)
    {
        try
        {
            var user = await _cf.GetUserAsync(handle);
            return Ok(user);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { error = "User not found", handle });
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    //-------------------------------------------summary-------------------------------------------
        /// <summary>
        /// this api return this response
        /// http://localhost:5243/api/v0.1/cf/user/orzdevinwang/summary

        /// 
        /// {
           /* "handle": "orzdevinwang",
            "rank": "legendary grandmaster",
            "currentRating": 3670,
            "maxRating": 3844,
            "country": "China",
            "registeredAt": 1583069392,
            "avatar": "https://userpic.codeforces.org/no-avatar.jpg"
            }
            */
        /// 
        /// <summary>


    
    [HttpGet("cf/user/{handle}/summary")]
    public async Task<IActionResult> GetUserSummary(string handle)
    {
        var u = await _cf.GetUserAsync(handle);
        var hist = await _cf.GetUserRatingAsync(handle);
        var current = hist.Count > 0 ? hist[^1].NewRating : u.Rating;

        var dto = new UserSummaryDto
        {
            Handle = u.Handle,
            Rank = u.Rank,
            CurrentRating = current,
            MaxRating = u.MaxRating,
            Country = u.Country,
            RegisteredAt = u.RegistrationTimeSeconds,
            Avatar = u.Avatar
        };
        return Ok(dto);
    }


    //------------------------------------------------------ratings api--------------------------------------------//

    // this api fetches all the rating and the new rating of the user off all the completed contest 

    /* 

    
  {
    "contestId": 1316,
    "contestName": "CodeCraft-20 (Div. 2)",
    "handle": "orzdevinwang",
    "rank": 6247,
    "ratingUpdateTimeSeconds": 1583339700,
    "oldRating": 0,
    "newRating": 1387
  },
  {
    "contestId": 1325,
    "contestName": "Codeforces Round 628 (Div. 2)",
    "handle": "orzdevinwang",
    "rank": 4982,
    "ratingUpdateTimeSeconds": 1584203700,
    "oldRating": 1387,
    "newRating": 1367
  },
    
    */
    [HttpGet("cf/user/{handle}/rating")]
    public async Task<IActionResult> GetUserRating(string handle)
    {
        var rating = await _cf.GetUserRatingAsync(handle);
        return Ok(rating);
    }


    //---------------------------------------------------------------------------------------------//


    /// <summary>
    /// 
    /// 
    /// this api fetches the submmison of the user of the contest and the some detials of his at that submission
    /// 
    /// 
  
  /*{
    "id": 332907036,
    "contestId": 2127,
    "creationTimeSeconds": 1754588081,
    "problem": {
      "contestId": 2127,
      "index": "G2",
      "name": "Inter Active (Hard Version)",
      "rating": 3500
    },
    "programmingLanguage": "C++17 (GCC 7-32)",
    "verdict": "WRONG_ANSWER"
  },
  {
    "id": 332906943,
    "contestId": 2127,
    "creationTimeSeconds": 1754588075,
    "problem": {
      "contestId": 2127,
      "index": "G1",
      "name": "Inter Active (Easy Version)",
      "rating": 3400
    },
    "programmingLanguage": "C++17 (GCC 7-32)",
    "verdict": "WRONG_ANSWER"
  },
  {
    "id": 332859308,
    "contestId": 2127,
    "creationTimeSeconds": 1754581911,
    "problem": {
      "contestId": 2127,
      "index": "H",
      "name": "23 Rises Again",
      "rating": 3100
    },
    "programmingLanguage": "C++17 (GCC 7-32)",
    "verdict": "OK"
  },
  {
    "id": 332841731,
    "contestId": 2127,
    "creationTimeSeconds": 1754580294,
    "problem": {
      "contestId": 2127,
      "index": "F",
      "name": "Hamed and AghaBalaSar",
      "rating": 2800
    },
    "programmingLanguage": "C++17 (GCC 7-32)",
    "verdict": "OK"
  },
  {
    "id": 332839716,
    "contestId": 2127,
    "creationTimeSeconds": 1754580137,
    "problem": {
      "contestId": 2127,
      "index": "F",
      "name": "Hamed and AghaBalaSar",
      "rating": 2800
    },
    "programmingLanguage": "C++17 (GCC 7-32)",
    "verdict": "WRONG_ANSWER"
  },
  {
    "id": 332827362,
    "contestId": 2127,
    "creationTimeSeconds": 1754579205,
    "problem": {
      "contestId": 2127,
      "index": "E",
      "name": "Ancient Tree",
      "rating": 2100
    },
    "programmingLanguage": "C++17 (GCC 7-32)",
    "verdict": "OK"
  },
  */
    /// 
    /// </summary>
    

    [HttpGet("cf/user/{handle}/submissions")]
    public async Task<IActionResult> GetUserSubmissions(
        string handle,
        [FromQuery] int from = 1,
        [FromQuery] int count = 100)
    {
        var subs = await _cf.GetUserSubmissionsAsync(handle, from, Math.Clamp(count, 1, 1000));
        return Ok(subs);
    }
}
