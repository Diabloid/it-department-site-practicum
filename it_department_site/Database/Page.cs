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
        public string ContentJson // ← Зберігається в базу
        {
            get => JsonSerializer.Serialize(Content, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            set => Content = JsonSerializer.Deserialize<ContentSections>(value) ?? new ContentSections();
        }
        public string LayoutType { get; set; } // <- нове поле
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

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

            if (!string.IsNullOrWhiteSpace(contentJson))
            {
                Content = System.Text.Json.JsonSerializer.Deserialize<ContentSections>(contentJson) ?? new ContentSections();
            }
            else
            {
                Content = new ContentSections();
            }
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
