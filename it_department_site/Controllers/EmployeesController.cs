using it_department_site.Services;
using it_department_site.Services.Interfaces;
using it_department_site.Entities;
using it_department_site.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace it_department_site.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IGoogleDriveService _driveService;

        public EmployeesController(IEmployeeService employeeService, IGoogleDriveService driveService)
        {
            _employeeService = employeeService;
            _driveService = driveService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllAsync();
            return View(employees.Select(ToViewModel).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            var viewModel = ToViewModel(employee);
            viewModel.Certificates = await LoadCertificatesAsync(employee, id);

            return View(viewModel);
        }

        [HttpGet("/Employees/CertificatePreview/{fileId}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> CertificatePreview(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId) ||
                !await _employeeService.IsPublishedDriveFileAsync(fileId))
            {
                return NotFound();
            }

            var preview = await _driveService.GetPreviewStreamAsync(fileId);
            if (preview == null)
                return NotFound();

            return File(preview.Value.Stream, preview.Value.ContentType);
        }

        private async Task<List<EmployeeCertificateViewModel>> LoadCertificatesAsync(Employee employee, int id)
        {
            var certificates = await _employeeService.GetCertificatesAsync(id);
            return certificates
                .Select(ToCertificateViewModel)
                .ToList();
        }

        private static EmployeeCertificateViewModel ToCertificateViewModel(EmployeeCertificate certificate)
        {
            var fileId = !string.IsNullOrWhiteSpace(certificate.DriveFileId)
                ? certificate.DriveFileId
                : ExtractDriveFileId(certificate.PhotoUrl);

            var isPdf = GoogleDriveFileHelper.IsPdfFromMimeOrUrl(
                certificate.MimeType,
                certificate.PhotoUrl,
                certificate.FileName);

            var previewUrl = GoogleDriveFileHelper.ResolvePreviewUrl(
                fileId,
                certificate.MimeType,
                certificate.FileName,
                certificate.PhotoUrl);

            return new EmployeeCertificateViewModel
            {
                LinkUrl = certificate.PhotoUrl,
                PreviewUrl = previewUrl,
                FileName = certificate.FileName ?? string.Empty,
                IsPdf = isPdf
            };
        }

        private static string? ExtractDriveFileId(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var patterns = new[]
            {
                @"file/d/([a-zA-Z0-9_-]+)",
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

        private static EmployeeInfoViewModel ToViewModel(Employee employee) => new()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            PatronymicName = employee.PatronymicName ?? string.Empty,
            Specialization = employee.Specialization ?? string.Empty,
            PhotoUrl = string.IsNullOrWhiteSpace(employee.PhotoUrl)
                ? "/images/banner_photo_2.jpg"
                : employee.PhotoUrl,
            Email = employee.Email ?? string.Empty,
            Bio = employee.Bio ?? string.Empty
        };
    }
}
