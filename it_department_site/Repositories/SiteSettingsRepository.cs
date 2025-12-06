namespace it_department_site.Repositories
{
    public class SiteSettingsRepository : ISiteSettingsRepository
    {
        private readonly IDbConnection _db;

        public SiteSettingsRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<SiteSettings> GetAsync()
        {
            var settings = await _db.QueryFirstOrDefaultAsync<SiteSettings>("SELECT * FROM site_settings WHERE Id = 1");

            if (settings == null)
            {
                return new SiteSettings
                {
                    Id = 1,
                    HeaderHtml = "<header></header>",
                    FooterHtml = "<footer></footer>",
                    CustomCss = ""
                };
            }
            return settings;
        }

        public async Task SaveAsync(SiteSettings settings)
        {
            var sql = @"
                UPDATE site_settings SET 
                    HeaderHtml = @HeaderHtml, 
                    HeaderCss = @HeaderCss, 
                    FooterHtml = @FooterHtml, 
                    FooterCss = @FooterCss, 
                    CustomCss = @CustomCss, 
                    SiteTitle = @SiteTitle 
                WHERE Id = 1";

            await _db.ExecuteAsync(sql, settings);
        }
    }
}