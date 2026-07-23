namespace it_department_site.Services.Interfaces;

public record DriveFolderInfo(string FolderId, string Name, string FolderUrl);

public record DriveFileInfo(string FileId, string Name, string MimeType, string WebViewLink, string PreviewUrl);

public interface IGoogleDriveService
{
    bool IsConfigured { get; }
    Task<IReadOnlyList<DriveFolderInfo>> ListSubfoldersAsync(string rootFolderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DriveFileInfo>> ListAllCertificateFilesAsync(string folderId, CancellationToken cancellationToken = default);
    Task<DriveFileInfo?> FindProfileImageAsync(string folderId, CancellationToken cancellationToken = default);
    Task<(Stream Stream, string ContentType)?> GetPreviewStreamAsync(string fileId, CancellationToken cancellationToken = default);
}
