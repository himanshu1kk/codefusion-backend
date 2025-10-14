using System.Text.Json.Serialization;

namespace CFFFusions.Models;

public class CfProblem
{
    [JsonPropertyName("contestId")] public int? ContestId { get; set; }
    [JsonPropertyName("index")] public string Index { get; set; } = default!;
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    [JsonPropertyName("rating")] public int? Rating { get; set; }
}

public class CfSubmission
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("contestId")] public int? ContestId { get; set; }
    [JsonPropertyName("creationTimeSeconds")] public long CreationTimeSeconds { get; set; }
    [JsonPropertyName("problem")] public CfProblem Problem { get; set; } = default!;
    [JsonPropertyName("programmingLanguage")] public string ProgrammingLanguage { get; set; } = default!;
    [JsonPropertyName("verdict")] public string? Verdict { get; set; }
}
