using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("discipline_links")]
    public class Link
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("url")]
        public string Url { get; set; } = string.Empty;

        [Required]
        [Column("order")]
        public int Order { get; set; }

        [Required]
        [Column("discipline_id")]
        public int DisciplineId { get; set; }

        [ForeignKey(nameof(DisciplineId))]
        public Discipline? Discipline { get; set; }
    }
}