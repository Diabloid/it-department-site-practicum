using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("posts")]
    public class Post
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("slug")]
        public string Slug { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; } = null!;

        [MaxLength(300)]
        [Column("excerpt")]
        public string? Excerpt { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; } = null!;

        [Column("thumbnail")]
        public string? Thumbnail { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("author_id")]
        public int AuthorId { get; set; }

        public User? Author { get; set; }
    }
}