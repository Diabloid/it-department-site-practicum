namespace it_department_site.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly ISiteSettingsRepository _repository;

        public SiteSettingsService(ISiteSettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<SiteSettings> GetAsync()
        {
            return await _repository.GetAsync();
        }

        public async Task SaveAsync(SiteSettings settings)
        {
            await _repository.SaveAsync(settings);
        }
    }
}