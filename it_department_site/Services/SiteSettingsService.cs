namespace it_department_site.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly ApplicationDbContext _context;

        public SiteSettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SiteSettings> GetAsync()
        {
            var settings = await _context.SiteSettings.FindAsync(1);

            if (settings == null)
            {
                return new SiteSettings
                {
                    Id = 0,
                    HeaderHtml = "<header></header>",
                    FooterHtml = "<footer></footer>",
                    HeaderCss = "",
                    FooterCss = "",
                    CustomCss = ""
                };
            }

            return settings;
        }

        public async Task SaveAsync(SiteSettings settings)
        {
            var existingSettings = await _context.SiteSettings.FindAsync(1); -> await _context.SiteSettings.FirstOrDefaultAsync();

            if (existingSettings == null)
            {
                _context.SiteSettings.Add(settings);
            }
            else
            {
                existingSettings.HeaderHtml = settings.HeaderHtml;
                existingSettings.HeaderCss = settings.HeaderCss;
                existingSettings.FooterHtml = settings.FooterHtml;
                existingSettings.FooterCss = settings.FooterCss;
                existingSettings.CustomCss = settings.CustomCss;
                existingSettings.SiteTitle = settings.SiteTitle;
                existingSettings.SiteLogoUrl = settings.SiteLogoUrl;
            }

            await _context.SaveChangesAsync();
        }
    }
}
