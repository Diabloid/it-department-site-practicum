namespace it_department_site.Models
{
    public class FavoritePageViewModel
    {
        public int PageId { get; set; }
        public string PageTitle { get; set; }
        public string PageSlug { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
    }
}
