using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("partners")]
    public class Partner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("logo_file_name")]
        public string LogoFileName { get; set; }

        [Required]
        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [MaxLength(255)]
        [Column("link")]
        public string Link { get; set; }
    }
}