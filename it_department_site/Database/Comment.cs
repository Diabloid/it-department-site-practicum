namespace it_department_site.Database
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PageId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } // Для виводу
    }

}
