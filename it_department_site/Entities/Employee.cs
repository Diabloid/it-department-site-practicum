using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Entities
{
    
    [Table("employees")]
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
        [Column("specialization")]
        public string Specialization { get; set; }

        [Required]
        [Column("bio")]
        public string Bio { get; set; }

        [Required]
        [Column("photo_url")]
        public string PhotoUrl { get; set; }
    }
}
