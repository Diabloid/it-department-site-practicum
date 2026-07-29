using System.Text.RegularExpressions;

namespace it_department_site.Services;

public static class GoogleDriveIdHelper
{
    public static string? ExtractFolderId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        var patterns = new[]
        {
            @"drive\.google\.com/drive/folders/([a-zA-Z0-9_-]+)",
            @"[?&]id=([a-zA-Z0-9_-]+)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(url, pattern);
            if (match.Success && match.Groups.Count > 1)
                return match.Groups[1].Value;
        }

        return null;
    }

    public static string BuildFolderUrl(string folderId) =>
        $"https://drive.google.com/drive/folders/{folderId}";

    public static string BuildFileViewUrl(string fileId) =>
        $"https://drive.google.com/file/d/{fileId}/view";

    public static string BuildThumbnailUrl(string fileId, int maxWidth = 1200) =>
        $"https://drive.google.com/thumbnail?id={fileId}&sz=w{maxWidth}";

    public static string BuildProfileThumbnailUrl(string fileId) =>
        BuildThumbnailUrl(fileId, 800);
}
