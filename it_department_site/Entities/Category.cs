using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
 
        [Required]
        [MaxLength(50)]
        [Column("name")]
        public required string Name { get; set; }
    }
}
