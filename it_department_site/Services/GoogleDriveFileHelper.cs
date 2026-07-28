using System.Text.RegularExpressions;
using it_department_site.Services.Interfaces;

namespace it_department_site.Services;

public static class GoogleDriveFileHelper
{
    public const string PdfIconUrl = "https://www.gstatic.com/images/branding/product/1x/drive_2020q4_32dp.png";

    private static readonly string[] ImageExtensions =
    {
        ".png", ".jpg", ".jpeg", ".webp", ".gif", ".bmp", ".heic", ".heif", ".tif", ".tiff", ".svg"
    };

    private static readonly Regex ProfileNamePattern = new(
        @"^(photo|avatar|фото|portrait|profile|зображення)(\.|$|_)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static bool IsImageFile(string? fileName, string? mimeType)
    {
        if (!string.IsNullOrWhiteSpace(mimeType) &&
            mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!string.IsNullOrWhiteSpace(mimeType) &&
            mimeType.Equals("application/vnd.google-apps.photo", StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.IsNullOrWhiteSpace(fileName)) return false;
        var ext = Path.GetExtension(fileName);
        return ImageExtensions.Any(e => ext.Equals(e, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsPdfFile(string? fileName, string? mimeType)
    {
        if (!string.IsNullOrWhiteSpace(mimeType) &&
            mimeType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.IsNullOrWhiteSpace(fileName)) return false;
        return Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCertificateFile(string? fileName, string? mimeType) =>
        IsImageFile(fileName, mimeType) || IsPdfFile(fileName, mimeType);

    public static bool IsProfilePhotoFile(string? fileName) =>
        !string.IsNullOrWhiteSpace(fileName) &&
        ProfileNamePattern.IsMatch(Path.GetFileNameWithoutExtension(fileName)) &&
        !IsPdfFile(fileName, null);

    public static bool IsCertificateCandidate(DriveFileInfo file) =>
        IsCertificateFile(file.Name, file.MimeType) && !IsProfilePhotoFile(file.Name);

    public static bool IsPdf(DriveFileInfo file) => IsPdfFile(file.Name, file.MimeType);

    public static bool IsPdfFromMimeOrUrl(string? mimeType, string? url, string? fileName = null)
    {
        if (IsPdfFile(fileName, mimeType)) return true;
        if (!string.IsNullOrWhiteSpace(url) &&
            url.Contains(".pdf", StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    public static string BuildProxyPreviewUrl(string fileId) =>
        $"/Employees/CertificatePreview/{fileId}";

    public static string BuildPreviewUrl(string fileId, string? mimeType, string? fileName)
    {
        if (IsPdfFile(fileName, mimeType))
            return GoogleDriveIdHelper.BuildThumbnailUrl(fileId, 600);

        return GoogleDriveIdHelper.BuildThumbnailUrl(fileId);
    }

    public static string ResolvePreviewUrl(string? fileId, string? mimeType, string? fileName, string? fallbackLink)
    {
        if (!string.IsNullOrWhiteSpace(fileId))
            return BuildProxyPreviewUrl(fileId);

        if (IsPdfFromMimeOrUrl(mimeType, fallbackLink, fileName))
            return PdfIconUrl;

        return "https://www.gstatic.com/images/branding/product/2x/drive_2020q4_48dp.png";
    }
}
