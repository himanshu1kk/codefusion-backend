namespace CFFFusions.Models;

public class RefreshToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;

    public string Token { get; set; } = null!;

    public string FamilyId { get; set; } = null!; // same across rotations

    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}