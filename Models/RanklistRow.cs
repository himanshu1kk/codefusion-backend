namespace CFFFusions.Models
{
    public class RanklistRow
    {
        public int Rank { get; set; }
        public Party Party { get; set; }
        public float Points { get; set; }
        public int Penalty { get; set; }
        public int SuccessfulHackCount { get; set; }
        public int UnsuccessfulHackCount { get; set; }
        public List<ProblemResult> ProblemResults { get; set; }
        public int? LastSubmissionTimeSeconds { get; set; }
    }
}
