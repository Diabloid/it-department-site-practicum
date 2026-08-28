namespace it_department_site.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task CreateAsync(User user, string plainPassword);
        Task UpdateAsync(int id, User user, string? plainPassword);
        Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(int? excludeUserId = null);
    }
}