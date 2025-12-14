namespace CFFFusions.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int ContestId { get; set; }
        public int CreationTimeSeconds { get; set; }
        public Problem Problem { get; set; }
        public Party Author { get; set; }
        public string ProgrammingLanguage { get; set; }
        public string Verdict { get; set; }
        public string Testset { get; set; }
        public int PassedTestCount { get; set; }
        public int TimeConsumedMillis { get; set; }
        public int MemoryConsumedBytes { get; set; }
        public float Points { get; set; }
    }
}
