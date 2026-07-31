using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Entities
{
    [Index(nameof(Slug), IsUnique = true)]
    [Table("posts")]
    public class Post
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [MaxLength(150)]
        [Required]
        [Column("slug")]
        public required string Slug { get; set; }

        [MaxLength(200)]
        [Required]
        [Column("title")]
        public required string Title { get; set; }

        [MaxLength(300)]
        [Column("excerpt")]
        public string? Excerpt { get; set; }

        [Required]
        [Column("content", TypeName = "text")]
        public required string Content { get; set; }

        [Column("thumbnail")]
        public string? Thumbnail { get; set; }

        [Required]
        [Column("status")]
        public PostStatus Status { get; set; } = PostStatus.Draft;
    
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        [Column("author_id")]
        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public User? Author { get; set; }
    }
    
    public enum PostStatus
    {
        Draft,
        Published
    }
}
