using it_department_site.Database;

namespace it_department_site.Services
{
    public interface IAuthService
    {
        User? Login(string username, string password); // 🔽 Змінено bool на User?
        bool IsUsernameTaken(string username);
        void Register(string username, string email, string password);
        int GetUserId(string username); // 🔽 Цей метод більше не потрібен в контролері, але хай буде
    }
}
