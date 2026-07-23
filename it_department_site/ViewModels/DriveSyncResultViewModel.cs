namespace it_department_site.ViewModels;

public class DriveSyncResultViewModel
{
    public int MatchedCount { get; set; }
    public List<string> UnmatchedFolders { get; set; } = new();
    public List<string> EmployeesWithoutFolder { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
