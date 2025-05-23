using it_department_site.Database;
namespace it_department_site.Services
{
    public interface IPageService
    {
        Page? GetPageBySlug(string slug, bool isAdmin);
        Page? GetPageById(int id);
        List<Page> GetAllPages();
        void CreatePage(Page page);
        void UpdatePage(Page page);
        void DeletePage(int id);
    }
}
