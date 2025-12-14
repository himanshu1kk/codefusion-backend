namespace CFFFusions.Models
{
    public class Party
    {
        public int ContestId { get; set; }
        public List<Member> Members { get; set; }
        public string ParticipantType { get; set; } // E.g., CONTESTANT, VIRTUAL, etc.
        public int? TeamId { get; set; }
        public string TeamName { get; set; }
        public bool Ghost { get; set; }
        public int? Room { get; set; }
        public int StartTimeSeconds { get; set; }
    }
}
