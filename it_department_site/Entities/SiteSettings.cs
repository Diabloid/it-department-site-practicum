using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("site_settings")]
    public class SiteSettings
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("site_title")]
        public string SiteTitle { get; set; }

        [Column("header_html", TypeName = "text")]
        public string? HeaderHtml { get; set; }

        [Column("footer_html", TypeName = "text")]
        public string? FooterHtml { get; set; }

        [Column("header_css", TypeName = "text")]
        public string? HeaderCss { get; set; }

        [Column("footer_css", TypeName = "text")]
        public string? FooterCss { get; set; }

        [Column("custom_css", TypeName = "text")]
        public string? CustomCss { get; set; }

        [MaxLength(255)]
        [Column("logo_url")]
        public string? SiteLogoUrl { get; set; }
    }
}