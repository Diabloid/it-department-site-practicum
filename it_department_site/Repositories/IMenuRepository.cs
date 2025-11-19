using it_department_site.Database;

namespace it_department_site.Repositories
{
    public interface IMenuRepository
    {
        List<MenuItem> GetAll();
        void LinkPageToMenu(int pageId, int? parentId, string title);
        public MenuItem GetById(int id);
        public void Create(MenuItem item);
        public void Update(MenuItem item);
        public void Delete(int id);
    }

}
