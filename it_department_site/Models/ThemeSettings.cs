namespace it_department_site.Models
{
    public class ThemeSettings
    {
        public string Primary { get; set; } = "#0d6efd";
        public string Secondary { get; set; } = "#6c757d";
        public string Success { get; set; } = "#198754";
        public string Danger { get; set; } = "#dc3545";
        public string Warning { get; set; } = "#ffc107";
        public string Info { get; set; } = "#0dcaf0";
        public string Light { get; set; } = "#f8f9fa";
        public string Dark { get; set; } = "#212529";
        public string BodyBg { get; set; } = "#ffffff";
        public string BodyText { get; set; } = "#212529";
        public string LinkColor { get; set; } = "#0d6efd";
    }
    public static class AvailableThemes
    {
        public static Dictionary<string, string> Themes = new()
    {
        { "default", "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" },
        { "flatly", "https://cdn.jsdelivr.net/npm/bootswatch@5.3.0/dist/flatly/bootstrap.min.css" },
        { "darkly", "https://cdn.jsdelivr.net/npm/bootswatch@5.3.0/dist/darkly/bootstrap.min.css" },
        { "cyborg", "https://cdn.jsdelivr.net/npm/bootswatch@5.3.0/dist/cyborg/bootstrap.min.css" },
        { "journal", "https://cdn.jsdelivr.net/npm/bootswatch@5.3.0/dist/journal/bootstrap.min.css" }
    };
    }
}
