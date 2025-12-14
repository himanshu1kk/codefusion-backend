namespace CFFFusions.Models
{
    public class ContestStandings
    {
        public Contest Contest { get; set; }
        public List<Problem> Problems { get; set; }
        public List<RanklistRow> Rows { get; set; }
    }
}
