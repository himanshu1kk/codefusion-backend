namespace CFFFusions.Models
{
    public class Hack
    {
        public int Id { get; set; }
        public int CreationTimeSeconds { get; set; }
        public Party Hacker { get; set; }
        public Party Defender { get; set; }
        public string Verdict { get; set; }
        public Problem Problem { get; set; }
        public string Test { get; set; }
        public HackJudgeProtocol JudgeProtocol { get; set; }
    }
}
