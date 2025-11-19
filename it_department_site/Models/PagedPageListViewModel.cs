using it_department_site.Database;
namespace it_department_site.Models
{
    public class PagedPageListViewModel
    {
        public List<Page> Pages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
