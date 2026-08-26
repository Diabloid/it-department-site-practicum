using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Entities
{
    [Table("disciplines")]
    [Index(nameof(Name), IsUnique = true)]
    public class Discipline
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [Required]
        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Required]
        [Column("is_required")]
        public bool IsRequired { get; set; } = true;

        public ICollection<Link> ResourceLinks { get; set; } = new List<Link>();
    }
}