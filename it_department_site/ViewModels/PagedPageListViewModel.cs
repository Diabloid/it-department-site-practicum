namespace it_department_site.ViewModels
{
    public class PagedPageListViewModel
    {
        public List<Page> Pages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
