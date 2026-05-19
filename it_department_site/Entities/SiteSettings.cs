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

        [MaxLength(255)]
        [Column("SiteTitle")]
        public string? SiteTitle { get; set; }

        [Required]
        [Column("HeaderHtml")]
        public string HeaderHtml { get; set; }

        [Column("FooterHtml")]
        public string? FooterHtml { get; set; }

        [Required]
        [Column("HeaderCss")]
        public string HeaderCss { get; set; }

        [Required]
        [Column("FooterCss")]
        public string FooterCss { get; set; }

        [Required]
        [Column("CustomCss")]
        public string CustomCss { get; set; }

        [MaxLength(255)]
        [Column("SiteLogoUrl")]
        public string? SiteLogoUrl { get; set; }
    }
}
