namespace CFFFusions.Models;

public class TagStats
{
    public string Tag { get; set; } = default!;
    public int Attempts { get; set; }
    public int Accepted { get; set; }
    public int WrongAnswer { get; set; }
    public int TimeLimitExceeded { get; set; }

    public double Accuracy =>
        Attempts == 0 ? 0 : (double)Accepted / Attempts * 100;

    public string Level { get; set; } = default!;
}
