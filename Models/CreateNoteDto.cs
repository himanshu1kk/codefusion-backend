namespace CFFFusions.Models;

public class CreateNoteDto
{
    public int ContestId { get; set; }
    public string Index { get; set; } = default!;
    public string Notes { get; set; } = default!;
}