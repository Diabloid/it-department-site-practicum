namespace it_department_site.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveImage(IFormFile file, string folderName);
    }
}