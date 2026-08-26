using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("discipline_links")]
    public class DisciplineResourceLink
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public required string Title { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("url")]
        public required string Url { get; set; }

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
