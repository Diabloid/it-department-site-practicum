using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class SiteSettingsService(ApplicationDbContext dbContext) : ISiteSettingsService
    {
        public async Task<SiteSettings> GetAsync()
        {
            var settings = await dbContext.Set<SiteSettings>().AsNoTracking().FirstOrDefaultAsync();

            if (settings == null)
            {
                return new SiteSettings
                {
                    Id = 0,
                    SiteTitle = string.Empty
                };
            }

            return settings;
        }

        public async Task SaveAsync(SiteSettings settings)
        {
            var existingSettings = await dbContext.Set<SiteSettings>().FirstOrDefaultAsync();

            if (existingSettings == null)
            {
                dbContext.Set<SiteSettings>().Add(settings);
            }
            else
            {
                existingSettings.SiteTitle = settings.SiteTitle;
                existingSettings.SiteLogoUrl = settings.SiteLogoUrl;
                existingSettings.HomePageId = settings.HomePageId;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}