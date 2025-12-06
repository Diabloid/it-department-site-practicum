namespace it_department_site.Repositories.Interfaces
{
    public interface ISiteSettingsRepository
    {
        Task<SiteSettings> GetAsync();
        Task SaveAsync(SiteSettings settings);
    }
}