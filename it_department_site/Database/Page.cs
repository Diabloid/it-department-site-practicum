using System.Text.Encodings.Web;
using System.Text.Json;

namespace it_department_site.Database
{
    public class Page
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public PageStatus Status { get; set; }
        public ContentSections Content { get; set; }
        public string ContentJson { get; set; } = "";
        public string LayoutType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool AllowComments { get; set; } = true; // ← Нове поле
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
