namespace it_department_site.Repositories
{
    public interface IAuthRepository
    {
        bool ValidateCredentials(string username, string password);
    }

}
