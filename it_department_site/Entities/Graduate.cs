using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("graduates")]
    public class Graduate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        [Column("first_name")]
        public required string FirstName { get; set; }

        [MaxLength(100)]
        [Required]
        [Column("last_name")]
        public required string LastName { get; set; }

        [Required]
        [Column("graduation_year")]
        public int GraduationYear { get; set; }

        [Column("specialization")]
        public string? Specialization { get; set; }

        [Required]
        [Column("photo_url")]
        public required string PhotoUrl { get; set; }

        [Column("job_description")]
        public string? JobDescription { get; set; }
        
        [Required]
        [Column("testimonial")]
        public required string Testimonial { get; set; }
    }
}