namespace it_department_site.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
