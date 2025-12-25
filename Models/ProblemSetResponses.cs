using System.Text.Json.Serialization;

namespace CFFFusions.Models;

public class ProblemSetResponses
{
    [JsonPropertyName("problems")]
    public List<Problem> Problems { get; set; } = [];
}
