using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("_password")]
        public string Password { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}