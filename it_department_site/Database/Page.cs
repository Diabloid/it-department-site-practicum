namespace it_department_site.Database
{
    public class Page
    {
        private int Id;
        private string Title;
        private string Slug;
        private PageStatus Status;
        private string Content;
        private DateTime CreatedAt;
        private DateTime UpdatedAt;

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
