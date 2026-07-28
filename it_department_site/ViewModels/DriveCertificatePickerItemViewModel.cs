namespace it_department_site.ViewModels;

public class DriveCertificatePickerItemViewModel
{
    public string FileId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PreviewUrl { get; set; } = string.Empty;
    public string WebViewLink { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public bool IsPdf { get; set; }
    public bool IsSelected { get; set; }
    public int SortOrder { get; set; }
}
