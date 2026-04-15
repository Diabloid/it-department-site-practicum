namespace it_department_site.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> FindUserByUsernameAsync(string username);
        Task<int> GetUserIdAsync(string username);
    }
}