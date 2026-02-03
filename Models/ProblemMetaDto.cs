namespace CFFFusions.Models;

public class ProblemMetaDto
{
    public int ContestId { get; set; }
    public string Index { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int? Rating { get; set; }
    public List<string> Tags { get; set; } = [];
}
