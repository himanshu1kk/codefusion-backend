namespace CFFFusions.Models
{
    public class Contest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // E.g., CF, IOI, ICPC
        public string Phase { get; set; } // E.g., BEFORE, CODING, PENDING_SYSTEM_TEST, SYSTEM_TEST, FINISHED
        public bool Frozen { get; set; }
        public int DurationSeconds { get; set; }
        public int? FreezeDurationSeconds { get; set; }
        public int? StartTimeSeconds { get; set; }
        public int? RelativeTimeSeconds { get; set; }
        public string PreparedBy { get; set; } // Contest creator
        public string WebsiteUrl { get; set; }
        public string Description { get; set; }
        public int? Difficulty { get; set; }
        public string Kind { get; set; } // Human-readable contest type
        public string IcpcRegion { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Season { get; set; }
    }
}
