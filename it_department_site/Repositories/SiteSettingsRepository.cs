using Dapper;
using dbhelper;
using it_department_site.Database;
using it_department_site.Repositories;

namespace it_department_site.Repositories
{
    public class SiteSettingsRepository : ISiteSettingsRepository
    {
        private readonly Dbhelper _dbHelper;

        public SiteSettingsRepository(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public SiteSettings Get()
        {
            using var db = _dbHelper.GetConnection();
            var settings = db.QueryFirstOrDefault<SiteSettings>("SELECT * FROM site_settings WHERE Id = 1");

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

        public void Save(SiteSettings settings)
        {
            using var db = _dbHelper.GetConnection();
            var sql = @"
                UPDATE site_settings SET 
                    HeaderHtml = @HeaderHtml, 
                    HeaderCss = @HeaderCss, 
                    FooterHtml = @FooterHtml, 
                    FooterCss = @FooterCss, 
                    CustomCss = @CustomCss, 
                    SiteTitle = @SiteTitle 
                WHERE Id = 1";

            db.Execute(sql, settings);
        }
    }
}