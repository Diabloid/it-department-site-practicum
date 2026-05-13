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

        [Required]
        [MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [Column("graduation_year")]
        public int GraduationYear { get; set; }

        [Column("specialization")]
        public string Specialization { get; set; }

        [Column("photo_url")]
        public string PhotoUrl { get; set; }

        [Column("job_description")]
        public string JobDescription { get; set; }
        
        [Column("testimonial")]
        public string Testimonial { get; set; }
    }
}