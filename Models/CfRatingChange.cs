using System.Text.Json.Serialization;

namespace CFFFusions.Models;

public class CfRatingChange
{
    [JsonPropertyName("contestId")] public int ContestId { get; set; }
    [JsonPropertyName("contestName")] public string ContestName { get; set; } = default!;
    [JsonPropertyName("handle")] public string Handle { get; set; } = default!;
    [JsonPropertyName("rank")] public int Rank { get; set; }
    [JsonPropertyName("ratingUpdateTimeSeconds")] public long RatingUpdateTimeSeconds { get; set; }
    [JsonPropertyName("oldRating")] public int OldRating { get; set; }
    [JsonPropertyName("newRating")] public int NewRating { get; set; }
}
