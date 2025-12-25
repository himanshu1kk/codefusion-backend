namespace CFFFusions.Models;

public class SkillGapResponse
{
    public List<TagStats> Weak { get; set; } = [];
    public List<TagStats> Average { get; set; } = [];
    public List<TagStats> Strong { get; set; } = [];
}
