namespace CFFFusions.Models
{
    public class TagDistributionResponse
    {
        public DateTime GeneratedAtUtc { get; set; }
        public List<RatingBucketTagStats> Buckets { get; set; } = new();
    }
}