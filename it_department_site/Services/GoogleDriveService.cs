using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using it_department_site.Infrastructure;
using it_department_site.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace it_department_site.Services;

public class GoogleDriveService : IGoogleDriveService
{
    private readonly GoogleDriveOptions _options;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<GoogleDriveService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private DriveService? _driveService;
    private GoogleCredential? _credential;

    public GoogleDriveService(
        IOptions<GoogleDriveOptions> options,
        IWebHostEnvironment env,
        ILogger<GoogleDriveService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _env = env;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public bool IsConfigured => ResolveCredentialsPath() != null;

    public async Task<IReadOnlyList<DriveFolderInfo>> ListSubfoldersAsync(
        string rootFolderId,
        CancellationToken cancellationToken = default)
    {
        var service = GetDriveService();
        var request = service.Files.List();
        request.Q = $"'{rootFolderId}' in parents and mimeType='application/vnd.google-apps.folder' and trashed=false";
        request.Fields = "files(id, name)";
        request.PageSize = 200;
        request.SupportsAllDrives = true;
        request.IncludeItemsFromAllDrives = true;

        var result = await request.ExecuteAsync(cancellationToken);
        return (result.Files ?? new List<Google.Apis.Drive.v3.Data.File>())
            .Where(f => !string.IsNullOrWhiteSpace(f.Id) && !string.IsNullOrWhiteSpace(f.Name))
            .Select(f => new DriveFolderInfo(
                f.Id!,
                f.Name!.Trim(),
                GoogleDriveIdHelper.BuildFolderUrl(f.Id!)))
            .OrderBy(f => f.Name)
            .ToList();
    }

    public Task<IReadOnlyList<DriveFileInfo>> ListAllCertificateFilesAsync(
        string folderId,
        CancellationToken cancellationToken = default) =>
        ListFilesInFolderAsync(folderId, includeProfilePhotos: false, cancellationToken);

    public async Task<DriveFileInfo?> FindProfileImageAsync(
        string folderId,
        CancellationToken cancellationToken = default)
    {
        var files = await ListFilesInFolderAsync(folderId, includeProfilePhotos: true, cancellationToken);
        return files.FirstOrDefault(f => GoogleDriveFileHelper.IsProfilePhotoFile(f.Name));
    }

    public async Task<(Stream Stream, string ContentType)?> GetPreviewStreamAsync(
        string fileId,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(fileId)) return null;

        try
        {
            var service = GetDriveService();
            var metaRequest = service.Files.Get(fileId);
            metaRequest.Fields = "thumbnailLink, mimeType, name";
            metaRequest.SupportsAllDrives = true;
            var meta = await metaRequest.ExecuteAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(meta.ThumbnailLink))
            {
                var fromThumb = await DownloadAuthenticatedAsync(meta.ThumbnailLink, cancellationToken);
                if (fromThumb != null)
                    return fromThumb;
            }

            if (GoogleDriveFileHelper.IsImageFile(meta.Name, meta.MimeType))
            {
                var stream = new MemoryStream();
                var mediaRequest = service.Files.Get(fileId);
                mediaRequest.SupportsAllDrives = true;
                await mediaRequest.DownloadAsync(stream, cancellationToken);
                stream.Position = 0;
                var contentType = string.IsNullOrWhiteSpace(meta.MimeType) ? "image/jpeg" : meta.MimeType;
                return (stream, contentType);
            }

            var publicThumbUrl = GoogleDriveFileHelper.BuildPreviewUrl(fileId, meta.MimeType, meta.Name);
            var fromPublic = await DownloadAuthenticatedAsync(publicThumbUrl, cancellationToken);
            if (fromPublic != null)
                return fromPublic;

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load Drive preview for file {FileId}", fileId);
            return null;
        }
    }

    private async Task<IReadOnlyList<DriveFileInfo>> ListFilesInFolderAsync(
        string folderId,
        bool includeProfilePhotos,
        CancellationToken cancellationToken)
    {
        var service = GetDriveService();
        var request = service.Files.List();
        request.Q = $"'{folderId}' in parents and trashed=false";
        request.Fields = "files(id, name, mimeType, webViewLink, thumbnailLink)";
        request.PageSize = 200;
        request.SupportsAllDrives = true;
        request.IncludeItemsFromAllDrives = true;

        var result = await request.ExecuteAsync(cancellationToken);
        return (result.Files ?? new List<Google.Apis.Drive.v3.Data.File>())
            .Where(f => !string.IsNullOrWhiteSpace(f.Id))
            .Select(f =>
            {
                var fileId = f.Id!;
                var previewUrl = !string.IsNullOrWhiteSpace(f.ThumbnailLink)
                    ? f.ThumbnailLink
                    : GoogleDriveFileHelper.BuildPreviewUrl(fileId, f.MimeType, f.Name);

                return new DriveFileInfo(
                    fileId,
                    f.Name ?? "image",
                    f.MimeType ?? string.Empty,
                    string.IsNullOrWhiteSpace(f.WebViewLink)
                        ? GoogleDriveIdHelper.BuildFileViewUrl(fileId)
                        : f.WebViewLink!,
                    previewUrl);
            })
            .Where(f => includeProfilePhotos
                ? GoogleDriveFileHelper.IsImageFile(f.Name, f.MimeType)
                : GoogleDriveFileHelper.IsCertificateFile(f.Name, f.MimeType))
            .Where(f => includeProfilePhotos || !GoogleDriveFileHelper.IsProfilePhotoFile(f.Name))
            .OrderBy(f => f.Name)
            .Select(f => f with
            {
                PreviewUrl = GoogleDriveFileHelper.BuildProxyPreviewUrl(f.FileId)
            })
            .ToList();
    }

    private async Task<(Stream Stream, string ContentType)?> DownloadAuthenticatedAsync(
        string url,
        CancellationToken cancellationToken)
    {
        var token = await GetAccessTokenAsync(cancellationToken);

        var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
        return (new MemoryStream(bytes), contentType);
    }

    private DriveService GetDriveService()
    {
        if (_driveService != null) return _driveService;

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = GetCredential(),
            ApplicationName = "IT Department Site"
        });

        return _driveService;
    }

    private GoogleCredential GetCredential()
    {
        if (_credential != null) return _credential;

        var credPath = ResolveCredentialsPath()
            ?? throw new InvalidOperationException("Google Drive credentials are not configured.");

        using var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read);
        _credential = GoogleCredential.FromStream(stream)
            .CreateScoped(DriveService.Scope.DriveReadonly);

        return _credential;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var credential = GetCredential();
        if (credential.UnderlyingCredential is ServiceAccountCredential serviceAccount)
            return await serviceAccount.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);

        throw new InvalidOperationException("Unsupported Google credential type for Drive preview.");
    }

    private string? ResolveCredentialsPath()
    {
        if (string.IsNullOrWhiteSpace(_options.CredentialsPath)) return null;

        var path = _options.CredentialsPath;
        if (!Path.IsPathRooted(path))
            path = Path.Combine(_env.ContentRootPath, path);

        return File.Exists(path) ? path : null;
    }
}
