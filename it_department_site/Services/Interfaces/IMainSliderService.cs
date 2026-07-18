namespace it_department_site.Services.Interfaces
{
    public interface IMainSliderService
    {
        Task<List<MainSliderSlide>> GetAdminSlidesAsync();
        Task<List<MainSliderSlide>> GetDisplaySlidesAsync();
        Task UpdateSlideAsync(byte slot, IFormFile imageFile, string? altText);
        Task ClearSlideAsync(byte slot);
    }
}
