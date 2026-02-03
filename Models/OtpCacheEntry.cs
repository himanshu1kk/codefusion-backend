namespace CFFFusions.Models;

public class OtpCacheEntry
{
    public string Otp { get; set; } = null!;
    public DateTime Expiry { get; set; }
    public int Attempts { get; set; }
}
