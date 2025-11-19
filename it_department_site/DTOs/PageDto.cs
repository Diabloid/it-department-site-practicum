namespace it_department_site.DTOs
{
    public class PageDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Slug { get; set; }
        public string LayoutType { get; set; }
        public string MainContent { get; set; }
        public string LeftSidebar { get; set; }
        public string RightSidebar { get; set; }
        public bool AllowComments { get; set; }
        public int Views { get; set; }
    }
}
