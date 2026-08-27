using it_department_site.Entities;
using it_department_site.ViewModels;

namespace it_department_site.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task CreateAsync(Employee employee);
        Task UpdateAsync(int id, Employee employee);
        Task DeleteAsync(int id);
        Task<List<EmployeeCertificate>> GetCertificatesAsync(int employeeId);
        Task<Dictionary<int, int>> GetCertificateCountsAsync(IEnumerable<int> employeeIds);
        Task ApplyDrivePhotoFromFolderAsync(int employeeId);
        Task<List<DriveCertificatePickerItemViewModel>> GetDriveCertificatePickerItemsAsync(int employeeId);
        Task SetSelectedDriveCertificatesAsync(int employeeId, IReadOnlyList<string> selectedFileIds);
        Task<DriveSyncResultViewModel> SyncDriveFoldersFromRootAsync(string rootFolderId);
        Task<bool> IsPublishedDriveFileAsync(string fileId);
        Task<bool> IsFileInEmployeeDriveFolderAsync(int employeeId, string fileId);
    }
}
