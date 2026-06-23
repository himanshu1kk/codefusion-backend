namespace CFFFusions.Models
{
    public class RatingBucketTagStats
    {
        public string BucketLabel { get; set; }
        public int TotalTagOccurrences { get; set; }
        public int DistinctTagCount { get; set; }
        public List<TagFrequency> Tags { get; set; } = new();
    }
}