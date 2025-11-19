using it_department_site.Database;

namespace it_department_site.Repositories
{
    public interface IProfileRepository
    {
        User? GetUserInfo(int userId);
    }
}
