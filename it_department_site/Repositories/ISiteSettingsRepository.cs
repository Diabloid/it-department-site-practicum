using it_department_site.Database;
using it_department_site.Models;

namespace it_department_site.Repositories
{
    public interface ISiteSettingsRepository
    {
        SiteSettings Get();
        void Save(SiteSettings settings);
    }
}
