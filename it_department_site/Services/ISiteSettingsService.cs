using it_department_site.Database;

namespace it_department_site.Services
{
    public interface ISiteSettingsService
    {
        SiteSettings Get();
        void Save(SiteSettings settings);
    }
}
