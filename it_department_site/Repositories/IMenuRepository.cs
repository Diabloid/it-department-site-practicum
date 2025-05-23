using it_department_site.Database;

namespace it_department_site.Repositories
{
    public interface IMenuRepository
    {
        List<MenuItem> GetAll();
    }
}
