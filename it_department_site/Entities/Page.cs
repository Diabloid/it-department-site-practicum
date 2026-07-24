using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Entities
{
    [Index(nameof(Slug), IsUnique = true)]
    [Table("pages")]
    public class Page
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("slug")]
        public string Slug { get; set; }

        [Required]
        [Column("status")]
        public PageStatus Status { get; set; }

        [NotMapped]
        public ContentSections Content { get; set; }

        [Column("content", TypeName = "text")]
        public string ContentJson { get; set; } = "";

        [Required]
        [MaxLength(50)]
        [Column("layout_type")]
        public string LayoutType { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("views_count")]
        public int ViewsCount { get; set; }

        public Page() { }

        public Page(int id, string title, string slug, PageStatus status, string contentJson, string layoutType, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Title = title;
            Slug = slug;
            Status = status;
            LayoutType = layoutType;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ContentJson = contentJson;
            Content = new ContentSections();
        }
    }

    public enum PageStatus
    {
        Draft,
        Published
    }

    public class ContentSections
    {
        public string main { get; set; } = "";
        public string left { get; set; } = "";
        public string right { get; set; } = "";
    }
}
