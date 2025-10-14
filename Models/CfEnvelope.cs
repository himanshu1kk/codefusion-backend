using System.Text.Json.Serialization;

namespace CFFFusions.Models;

public class CfEnvelope<T>
{
    [JsonPropertyName("status")] public string Status { get; set; } = default!;
    [JsonPropertyName("comment")] public string? Comment { get; set; }
    [JsonPropertyName("result")] public T? Result { get; set; }
}
