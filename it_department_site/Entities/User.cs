using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Entities
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(EmployeeId), IsUnique = true)]
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public required string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("email")]
        public required string Email { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("employee_id")]
        public int? EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }
        
        [Required]
        [Column("roles", TypeName = "json")]
        public List<UserRole> Roles { get; set; } = new();

    }
 
    public enum UserRole
    {
        Author,
        Admin
    }
}