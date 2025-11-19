namespace it_department_site.Database
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PageId { get; set; }
        public string PageTitle { get; set; }
        public string PageSlug { get; set; }
    }
}
