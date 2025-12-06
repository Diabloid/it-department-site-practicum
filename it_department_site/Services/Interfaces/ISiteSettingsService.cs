namespace it_department_site.Services.Interfaces
{
    public interface ISiteSettingsService
    {
        Task<SiteSettings> GetAsync();
        Task SaveAsync(SiteSettings settings);
    }
}