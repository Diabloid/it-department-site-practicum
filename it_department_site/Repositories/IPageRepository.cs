using it_department_site.Database;
namespace it_department_site.Repositories
{
    public interface IPageRepository
    {
        Page? GetBySlug(string slug);
        Page? GetById(int id);
        List<Page> GetAll();
        void Create(Page page);
        void Update(Page page);
        void Delete(int id);
    }

}
