using Cff.Error.Exceptions;
using Cff.Error.Extensions;
using CFFFusions.Models;
using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;

namespace CFFFusions.Controllers
{
    [ApiController]
    [Route("api/v0.1")]
    public class CfProblemController : ControllerBase
    {
        private readonly IProblemClient _problemClient;

        public CfProblemController(IProblemClient problemClient)
        {
            _problemClient = problemClient;
        }


/*the reponse of the below api is in this way

{
  "page": 1,
  "pageSize": 5,
  "totalCount": 1556,
  "items": [
    {
      "contestId": 2138,
      "problemsetName": null,
      "index": "C2",
      "name": "Maple and Tree Beauty (Hard Version)",
      "type": "PROGRAMMING",
      "points": 750,
      "rating": 2000,
      "tags": [
        "bitmasks",
        "brute force",
        "dfs and similar",
        "dp",
        "fft",
        "trees"
      ]
    },
    {
      "contestId": 2131,
      "problemsetName": null,
      "index": "G",
      "name": "Wafu!",
      "type": "PROGRAMMING",
      "points": null,
      "rating": 2000,
      "tags": [
        "bitmasks",
        "brute force",
        "data structures",
        "dfs and similar",
        "dp",
        "math"
      ]
    },
    {
      "contestId": 2114,
      "problemsetName": null,
      "index": "F",
      "name": "Small Operations",
      "type": "PROGRAMMING",
      "points": null,
      "rating": 2000,
      "tags": [
        "binary search",
        "brute force",
        "dfs and similar",
        "dp",
        "math",
        "number theory",
        "sortings"
      ]
    },
    {
      "contestId": 2075,
      "problemsetName": null,
      "index": "D",
      "name": "Equalization",
      "type": "PROGRAMMING",
      "points": null,
      "rating": 2000,
      "tags": [
        "bitmasks",
        "brute force",
        "dp",
        "graphs",
        "math"
      ]
    },
    {
      "contestId": 2061,
      "problemsetName": null,
      "index": "E",
      "name": "Kevin and And",
      "type": "PROGRAMMING",
      "points": 2000,
      "rating": 2000,
      "tags": [
        "bitmasks",
        "brute force",
        "dp",
        "greedy",
        "math",
        "sortings"
      ]
    }
  ]
}
*/
[HttpGet("cf/problems")]
public async Task<IActionResult> GetProblems(
    [FromQuery] string? tag,
    [FromQuery] int? minRating,
    [FromQuery] int? maxRating,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
{
    tag ??= "dp";
    try
    {
        var result = await _problemClient.GetProblemsAsync(
            tag,
            minRating,
            maxRating,
            page,
            pageSize
        );

        return Ok(result);
    }
    catch (CffError err)
    {
        return err.ToActionResult();
    }
}
    }
}
