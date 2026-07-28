namespace it_department_site.Infrastructure;

public class GoogleDriveOptions
{
    public const string SectionName = "GoogleDrive";

    public string CredentialsPath { get; set; } = string.Empty;
    public string RootFolderId { get; set; } = "1SGYkmGwxpaREM2rGKv1H_hIOJ0HL9mu5";
    public int MaxCertificatesPerEmployee { get; set; } = 15;
}
