using it_department_site.Entities;

namespace it_department_site.ViewModels;

public class GraduateTimelinePageViewModel
{
    public List<GraduateYearGroupViewModel> YearGroups { get; set; } = new();
    public int TotalCount { get; set; }
}

public class GraduateYearGroupViewModel
{
    public int Year { get; set; }
    public List<Graduate> Graduates { get; set; } = new();
}

public static class GraduateTimelineBuilder
{
    public static GraduateTimelinePageViewModel Build(IEnumerable<Graduate> graduates)
    {
        var list = graduates?.ToList() ?? new List<Graduate>();
        var yearGroups = list
            .GroupBy(g => g.GraduationYear)
            .OrderByDescending(g => g.Key)
            .Select(g => new GraduateYearGroupViewModel
            {
                Year = g.Key,
                Graduates = g
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToList()
            })
            .ToList();

        return new GraduateTimelinePageViewModel
        {
            YearGroups = yearGroups,
            TotalCount = list.Count
        };
    }
}
