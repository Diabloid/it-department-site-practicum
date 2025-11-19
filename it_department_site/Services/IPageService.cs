using it_department_site.Database;
namespace it_department_site.Services
{
    public interface IPageService
    {
        Page? GetPageBySlug(string slug, bool isAdmin);
        Page? GetPageById(int id);
        List<Page> GetAllPages();
        int CreatePage(Page page);
        void UpdatePage(Page page);
        void DeletePage(int id);
        public List<Page> GetPaged(int b, int c);
        public int GetTotal(int p);
        List<Page> Search(string query);
        public bool HasUserViewedPage(int userId, int pageId);
        public void MarkPageAsViewed(int userId, int pageId);
        public int GetViews(int pageId);
    }
}
