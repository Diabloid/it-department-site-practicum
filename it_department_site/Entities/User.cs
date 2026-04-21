using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        [Column("_password")]
        public string Password { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}