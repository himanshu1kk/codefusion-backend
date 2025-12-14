namespace CFFFusions.Models
{
    public class RatingChange
    {
        public int ContestId { get; set; }
        public string ContestName { get; set; }
        public string Handle { get; set; }
        public int Rank { get; set; }
        public int RatingUpdateTimeSeconds { get; set; }
        public int OldRating { get; set; }
        public int NewRating { get; set; }
    }
}
