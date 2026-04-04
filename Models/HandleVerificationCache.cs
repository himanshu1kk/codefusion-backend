namespace CFFFusions.Models{
public class HandleVerificationCache
{
    public string UserId { get; set; } = null!;
    public string Handle { get; set; } = null!;
    public string Otp { get; set; } = null!;
    public int Attempts { get; set; } = 0;
    public long CreatedAt { get; set; }
}
}