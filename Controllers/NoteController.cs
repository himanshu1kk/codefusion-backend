using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;
using Cff.Error.Exceptions;
using Cff.Error.Extensions;
using System.Security.Claims;
using CFFFusions.Models;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/v0.1/cf/notes")]
public class NotesController : ControllerBase
{
    private readonly INotesService _service;

    public NotesController(INotesService service)
    {
        _service = service;
    }

    private string GetUserId()
        => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

    [HttpPost]
    public async Task<IActionResult> SaveNote([FromBody] CreateNoteDto dto)
    {
        try
        {
            // var userId = GetUserId();
            await _service.SaveNoteAsync("123", dto);
            return Ok();
        }
        catch (CffError err)
        {
            return err.ToActionResult();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNotes()
    {
        // var userId = GetUserId();
        return Ok(await _service.GetUserNotesAsync("123"));
    }

    [HttpGet("{contestId}/{index}")]
    public async Task<IActionResult> GetNote(int contestId, string index)
    {
        // var userId = GetUserId();
        
          var note = await _service.GetNoteAsync("123", contestId, index);

    if (note == null)
        return NotFound();

    return Ok(note);
    }
}