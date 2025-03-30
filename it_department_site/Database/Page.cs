namespace it_department_site.Database
{
    public class Page
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public PageStatus Status { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Page() { }

        public Page(int id, string title, string slug, PageStatus status, string content, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Title = title;
            Slug = slug;
            Status = status;
            Content = content;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public enum PageStatus
    {
        Draft,
        Published
    }
}
