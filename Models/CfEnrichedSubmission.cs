namespace CFFFusions.Models;

public class CfEnrichedSubmission
{
    public int ContestId { get; set; }
    public string Index { get; set; } = default!;
    public string Verdict { get; set; } = default!;
    public int? Rating { get; set; }
    public List<string> Tags { get; set; } = [];
}
