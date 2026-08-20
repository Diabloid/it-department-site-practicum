namespace it_department_site.Services;

using it_department_site.Entities;
using it_department_site.Infrastructure;
using it_department_site.Services;
using it_department_site.Services.Interfaces;
using it_department_site.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;
    private readonly IGoogleDriveService _driveService;
    private readonly GoogleDriveOptions _driveOptions;

    public EmployeeService(
        ApplicationDbContext context,
        IGoogleDriveService driveService,
        IOptions<GoogleDriveOptions> driveOptions)
    {
        _context = context;
        _driveService = driveService;
        _driveOptions = driveOptions.Value;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync() =>
        await _context.Employees.ToListAsync();

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _context.Employees
        .Include(e => e.Disciplines)
        .FirstOrDefaultAsync(g => g.Id == id);

    public async Task CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Employee employee)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return;

        existing.FirstName = employee.FirstName;
        existing.LastName = employee.LastName;
        existing.PatronymicName = employee.PatronymicName;
        existing.Specialization = employee.Specialization;
        existing.Bio = employee.Bio;
        existing.Email = employee.Email;
        existing.GoogleDriveFolderUrl = employee.GoogleDriveFolderUrl;

        if (!string.IsNullOrEmpty(employee.PhotoUrl))
            existing.PhotoUrl = employee.PhotoUrl;

        existing.Disciplines.Clear();
        if (employee.Disciplines != null && employee.Disciplines.Any())
        {
            foreach (var discipline in employee.Disciplines)
            {
                existing.Disciplines.Add(discipline);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return;

        var certificates = await _context.EmployeeCertificates
            .Where(c => c.EmployeeId == id)
            .ToListAsync();
        if (certificates.Count > 0)
            _context.EmployeeCertificates.RemoveRange(certificates);

        _context.Employees.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<List<EmployeeCertificate>> GetCertificatesAsync(int employeeId) =>
        await _context.EmployeeCertificates
            .Where(c => c.EmployeeId == employeeId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Id)
            .ToListAsync();

    public async Task<Dictionary<int, int>> GetCertificateCountsAsync(IEnumerable<int> employeeIds)
    {
        var ids = employeeIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<int, int>();

        return await _context.EmployeeCertificates
            .Where(c => ids.Contains(c.EmployeeId))
            .GroupBy(c => c.EmployeeId)
            .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EmployeeId, x => x.Count);
    }

    public async Task ApplyDrivePhotoFromFolderAsync(int employeeId)
    {
        if (!_driveService.IsConfigured) return;

        var employee = await GetByIdAsync(employeeId);
        if (employee == null || string.IsNullOrWhiteSpace(employee.GoogleDriveFolderUrl)) return;

        var folderId = GoogleDriveIdHelper.ExtractFolderId(employee.GoogleDriveFolderUrl);
        if (folderId == null) return;

        var profile = await _driveService.FindProfileImageAsync(folderId);
        if (profile == null) return;

        employee.PhotoUrl = GoogleDriveIdHelper.BuildProfileThumbnailUrl(profile.FileId);
        await _context.SaveChangesAsync();
    }

    public async Task<List<DriveCertificatePickerItemViewModel>> GetDriveCertificatePickerItemsAsync(int employeeId)
    {
        var employee = await GetByIdAsync(employeeId);
        if (employee == null || string.IsNullOrWhiteSpace(employee.GoogleDriveFolderUrl) || !_driveService.IsConfigured)
            return new List<DriveCertificatePickerItemViewModel>();

        var folderId = GoogleDriveIdHelper.ExtractFolderId(employee.GoogleDriveFolderUrl);
        if (folderId == null) return new List<DriveCertificatePickerItemViewModel>();

        var certificateFiles = (await _driveService.ListAllCertificateFilesAsync(folderId)).ToList();

        var selected = await GetCertificatesAsync(employeeId);
        var selectedByFileId = selected
            .Where(c => !string.IsNullOrWhiteSpace(c.DriveFileId))
            .ToDictionary(c => c.DriveFileId!, c => c.SortOrder);

        return certificateFiles.Select(f => new DriveCertificatePickerItemViewModel
        {
            FileId = f.FileId,
            Name = f.Name,
            PreviewUrl = $"/Admin/DriveFilePreview?employeeId={employeeId}&fileId={f.FileId}",
            WebViewLink = f.WebViewLink,
            MimeType = f.MimeType,
            IsPdf = GoogleDriveFileHelper.IsPdf(f),
            IsSelected = selectedByFileId.ContainsKey(f.FileId),
            SortOrder = selectedByFileId.TryGetValue(f.FileId, out var order) ? order : 0
        }).ToList();
    }

    public async Task SetSelectedDriveCertificatesAsync(int employeeId, IReadOnlyList<string> selectedFileIds)
    {
        var employee = await GetByIdAsync(employeeId);
        if (employee == null || string.IsNullOrWhiteSpace(employee.GoogleDriveFolderUrl)) return;

        var folderId = GoogleDriveIdHelper.ExtractFolderId(employee.GoogleDriveFolderUrl);
        if (folderId == null) return;

        var fileById = (await _driveService.ListAllCertificateFilesAsync(folderId))
            .ToDictionary(f => f.FileId);

        var ids = selectedFileIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .Take(_driveOptions.MaxCertificatesPerEmployee)
            .ToList();

        var existing = await _context.EmployeeCertificates
            .Where(c => c.EmployeeId == employeeId)
            .ToListAsync();
        if (existing.Count > 0)
            _context.EmployeeCertificates.RemoveRange(existing);

        var sortOrder = 0;
        foreach (var fileId in ids)
        {
            if (!fileById.TryGetValue(fileId, out var file)) continue;

            await _context.EmployeeCertificates.AddAsync(new EmployeeCertificate
            {
                EmployeeId = employeeId,
                DriveFileId = file.FileId,
                PhotoUrl = file.WebViewLink,
                MimeType = file.MimeType,
                FileName = file.Name,
                SortOrder = sortOrder++
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<DriveSyncResultViewModel> SyncDriveFoldersFromRootAsync(string rootFolderId)
    {
        var result = new DriveSyncResultViewModel();

        if (!_driveService.IsConfigured)
        {
            result.ErrorMessage = "Google Drive credentials are not configured.";
            return result;
        }

        try
        {
            var subfolders = await _driveService.ListSubfoldersAsync(rootFolderId);
            var employees = await _context.Employees.ToListAsync();

            foreach (var folder in subfolders)
            {
                var employee = employees.FirstOrDefault(e =>
                    string.Equals(e.LastName.Trim(), folder.Name, StringComparison.OrdinalIgnoreCase));

                if (employee == null)
                {
                    result.UnmatchedFolders.Add(folder.Name);
                    continue;
                }

                employee.GoogleDriveFolderUrl = folder.FolderUrl;
                result.MatchedCount++;
                await ApplyDrivePhotoFromFolderAsync(employee.Id);
            }

            await _context.SaveChangesAsync();

            result.EmployeesWithoutFolder = employees
                .Where(e => string.IsNullOrWhiteSpace(e.GoogleDriveFolderUrl))
                .Select(e => $"{e.LastName} {e.FirstName}")
                .ToList();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public Task<bool> IsPublishedDriveFileAsync(string fileId) =>
        _context.EmployeeCertificates.AnyAsync(c => c.DriveFileId == fileId);

    public async Task<bool> IsFileInEmployeeDriveFolderAsync(int employeeId, string fileId)
    {
        if (!_driveService.IsConfigured || string.IsNullOrWhiteSpace(fileId)) return false;

        var employee = await GetByIdAsync(employeeId);
        if (employee == null || string.IsNullOrWhiteSpace(employee.GoogleDriveFolderUrl)) return false;

        var folderId = GoogleDriveIdHelper.ExtractFolderId(employee.GoogleDriveFolderUrl);
        if (folderId == null) return false;

        var files = await _driveService.ListAllCertificateFilesAsync(folderId);
        return files.Any(f => f.FileId == fileId);
    }
}
