using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Entities
{
    
    [Table("employees")]
    [Index(nameof(Email), IsUnique = true)]
    public class Employee
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(220)]
        [Column("firstname")]
        public string FirstName { get; set; }

         [Required]
        [MaxLength(220)]
        [Column("lastname")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(220)]
        [Column("patronymic_name")]
        public string PatronymicName { get; set; }
    
        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("specialization", TypeName = "text")]
        public string Specialization { get; set; }

        [Required]
        [Column("bio", TypeName = "text")]
        public string Bio { get; set; }

        [Required]
        [Column("photo_url")]
        public string PhotoUrl { get; set; }

        [MaxLength(500)]
        [Column("google_drive_folder_url")]
        public string? GoogleDriveFolderUrl { get; set; }

        public ICollection<EmployeeCertificate> Certificates { get; set; } = new List<EmployeeCertificate>();

        public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
    }
}
