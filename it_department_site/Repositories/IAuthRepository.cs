using it_department_site.Database;

namespace it_department_site.Repositories
{
    public interface IAuthRepository
    {
        User? ValidateCredentials(string username, string password);
        bool IsUsernameTaken(string username);
        void RegisterUser(string username, string email, string password);
        int GetUserId(string username);
    }
}
