namespace it_department_site.Repositories.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
