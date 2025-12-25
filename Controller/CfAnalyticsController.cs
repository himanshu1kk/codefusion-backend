using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;
using Cff.Error.Exceptions;
using Cff.Error.Extensions;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/v0.1/cf/analytics")]
public class CfAnalyticsController : ControllerBase
{
    private readonly ICfAnalyticsService _analytics;

    public CfAnalyticsController(ICfAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    /// <summary>
    /// Returns user submissions enriched with problem tags and rating
    /// </summary>
    /// Example:
    /// GET /api/v0.1/cf/analytics/user/orzdevinwang/enriched?limit=500
   [HttpGet("user/{handle}/skill-gap")]
public async Task<IActionResult> GetSkillGap(
    string handle,
    [FromQuery] int limit = 500)
{
    try
    {
        var result = await _analytics.GetSkillGapAsync(
            handle,
            Math.Clamp(limit, 1, 1000)
        );

        return Ok(result);
    }
    catch (CffError err)
    {
        return err.ToActionResult();
    }
}
}