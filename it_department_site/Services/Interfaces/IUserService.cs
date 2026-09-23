namespace it_department_site.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task CreateAsync(User user);
        Task UpdateAsync(int id, User user);
        Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(int? excludeUserId = null);
        Task<User?> FindUserByEmailAsync(string email);
    }
}