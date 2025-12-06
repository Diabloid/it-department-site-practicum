namespace it_department_site.Database
{
    public class SiteSettings
    {
        public int Id { get; set; }

        public string? SiteTitle { get; set; } // ✅ назва сайту

        public string HeaderHtml { get; set; }
        public string? FooterHtml { get; set; } // ✅ футер
        public string HeaderCss { get; set; } // <== нове поле
        public string FooterCss { get; set; } // <== нове поле
        public string CustomCss { get; set; } // 🆕 нове поле


    }

}
