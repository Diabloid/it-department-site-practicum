using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("pages")]
    public class Page
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [MaxLength(50)]
        [Column("title")]
        public string Title { get; set; }

        [MaxLength(100)]
        [Column("slug")]
        public string Slug { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public PageStatus Status { get; set; }

        [NotMapped]
        public ContentSections Content { get; set; }

        [MaxLength(1000)]
        [Column("content_json")]
        public string ContentJson { get; set; } = "";

        [MaxLength(50)]
        [Column("layout_type")]
        public string LayoutType { get; set; }

        [MaxLength(20)]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [MaxLength(20)]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [MaxLength(20)]
        [Column("allow_comments")]
        public bool AllowComments { get; set; } = true; // ← Нове поле

        [MaxLength(20)]
        [Column("views")]
        public int Views { get; set; }

        public Page() { }

        public Page(int id, string title, string slug, PageStatus status, string contentJson, string layoutType, DateTime createdAt, DateTime updatedAt, bool allowComments = true)
        {
            Id = id;
            Title = title;
            Slug = slug;
            Status = status;
            LayoutType = layoutType;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            AllowComments = allowComments;

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
