using it_department_site.Database;
using it_department_site.Repositories;

namespace it_department_site.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly ISiteSettingsRepository _repository;

        public SiteSettingsService(ISiteSettingsRepository repository)
        {
            _repository = repository;
        }

        public SiteSettings Get()
        {
            return _repository.Get();
        }

        public void Save(SiteSettings settings)
        {
            _repository.Save(settings);
        }
    }
}
