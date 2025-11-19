using it_department_site.Database;
using dbhelper;
using System.Data;
using it_department_site.Models;

namespace it_department_site.Repositories
{
    public class SiteSettingsRepository : ISiteSettingsRepository
    {
        private readonly Dbhelper _db;

        public SiteSettingsRepository(Dbhelper db)
        {
            _db = db;
        }

        public SiteSettings Get()
        {
            var dt = _db.ExecuteQuery("SELECT * FROM SiteSettings WHERE Id = 1");

            if (dt.Rows.Count == 0)
            {
                return new SiteSettings
                {
                    Id = 1,
                    HeaderHtml = "<header></header>",
                    FooterHtml = "<footer></footer>",
                    CustomCss = ""
                };
            }

            var row = dt.Rows[0];
            return new SiteSettings
            {
                Id = (int)row["Id"],
                HeaderHtml = row["HeaderHtml"].ToString(),
                HeaderCss = row["HeaderCss"].ToString(),       // 👈 Додано
                FooterHtml = row["FooterHtml"].ToString(),
                FooterCss = row["FooterCss"].ToString(),
                CustomCss = row["CustomCss"].ToString(),
                SelectedTheme = row["SelectedTheme"]?.ToString(),
                SiteTitle = row["SiteTitle"]?.ToString()
            };
        }

        public void Save(SiteSettings settings)
        {
            var parameters = new Dictionary<string, object>
    {
        { "@HeaderHtml", settings.HeaderHtml },
        { "@HeaderCss", settings.HeaderCss },      // 👈 Додано
        { "@FooterHtml", settings.FooterHtml },
        { "@FooterCss", settings.FooterCss },
        { "@CustomCss", settings.CustomCss }, // 🆕 нове поле
        { "@SelectedTheme", settings.SelectedTheme },
        {"@SiteTitle", settings.SiteTitle }
    };

            _db.ExecuteNonQuery(
                "UPDATE SiteSettings SET HeaderHtml = @HeaderHtml, HeaderCss = @HeaderCss, FooterHtml = @FooterHtml, FooterCss = @FooterCss, CustomCss = @CustomCss, SelectedTheme = @SelectedTheme, SiteTitle = @SiteTitle WHERE Id = 1",
                parameters
            );
        }
    }
}
