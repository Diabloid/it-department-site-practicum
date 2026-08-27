using it_department_site.Services.Interfaces;

namespace it_department_site.Services
{
    public class FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger) : IFileStorageService
    {
        public async Task<string> SaveImage(IFormFile file, string folderName)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var rootPath = Path.Combine(env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
                logger.LogInformation("Created directory for uploads at {RootPath}", rootPath);
            }
            var filePath = Path.Combine(rootPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                logger.LogInformation("Saving file to {FilePath}", filePath);
                await file.CopyToAsync(stream);
            }
            logger.LogInformation("File saved successfully: {FileName}", fileName);
            return $"/uploads/{folderName}/{fileName}";
        }
    }
}