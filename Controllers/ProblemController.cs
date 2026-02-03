using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;
using Cff.Error.Exceptions;
using Cff.Error.Extensions;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/v0.1/cf/problems")]
public class ProblemController : ControllerBase
{
    private readonly IProblemMetaService _service;

    public ProblemController(IProblemMetaService service)
    {
        _service = service;
    }

    [HttpGet("meta")]
    public async Task<IActionResult> GetProblemMeta(
        [FromQuery] int contestId,
        [FromQuery] string index)
    {
        try
        {
            var result = await _service.GetProblemMetaAsync(contestId, index);
            return Ok(result);
        }
        catch (CffError err)
        {
            return err.ToActionResult();
        }
    }
}
