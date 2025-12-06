namespace it_department_site.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> ValidateCredentialsAsync(string username, string password);
        Task<int> GetUserIdAsync(string username);
    }
}