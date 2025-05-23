namespace it_department_site.Services
{
    public interface IAuthService
    {
        bool Login(string username, string password);
    }
}
