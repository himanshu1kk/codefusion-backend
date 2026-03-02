using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CFFFusions.Models;

public class Note
{
  [BsonId]
[BsonRepresentation(BsonType.ObjectId)]
public string? Id { get; set; }

    public string UserId { get; set; } = default!;
    public int ContestId { get; set; }
    public string Index { get; set; } = default!;

    public string ProblemName { get; set; } = default!;
    public List<string> Tags { get; set; } = [];
    public int? Rating { get; set; }

    public string Notes { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

   
}