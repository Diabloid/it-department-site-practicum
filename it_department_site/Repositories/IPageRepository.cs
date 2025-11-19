using it_department_site.Database;
namespace it_department_site.Repositories
{
    public interface IPageRepository
    {
        Page? GetBySlug(string slug);
        Page? GetById(int id);
        List<Page> GetAll();
        int Create(Page page);
        void Update(Page page);
        void Delete(int id);
        public List<Page> GetPaged(int b, int c);
        public int GetTotal(int p);
        List<Page> Search(string query);
        bool HasPageView(int userId, int pageId);
        void AddPageView(int userId, int pageId);
        void IncrementViews(int pageId);
        int GetViews(int pageId);
    }

}
