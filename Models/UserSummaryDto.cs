namespace CFFFusions.Models;

public class UserSummaryDto
{
    public string Handle { get; init; } = default!;
    public string Rank { get; init; } = default!;
    public int CurrentRating { get; init; }
    public int MaxRating { get; init; }
    public string? Country { get; init; }
    public long RegisteredAt { get; init; }
    public string Avatar { get; init; } = default!;
}
