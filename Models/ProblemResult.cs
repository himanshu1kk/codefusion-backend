namespace CFFFusions.Models
{
    public class ProblemResult
    {
        public float Points { get; set; }
        public int Penalty { get; set; }
        public int RejectedAttemptCount { get; set; }
        public string Type { get; set; } // PRELIMINARY or FINAL
        public int BestSubmissionTimeSeconds { get; set; }
    }
}
