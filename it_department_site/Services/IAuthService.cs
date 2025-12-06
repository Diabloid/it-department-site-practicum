using it_department_site.Database;

namespace it_department_site.Services
{
    public interface IAuthService
    {
        User? Login(string username, string password); // 🔽 Змінено bool на User?
        int GetUserId(string username); // 🔽 Цей метод більше не потрібен в контролері, але хай буде
    }
}
