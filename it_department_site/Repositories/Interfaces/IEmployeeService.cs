namespace it_department_site.Repositories.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task CreateAsync(Employee employee);
        Task UpdateAsync(int id, Employee employee);
        Task DeleteAsync(int id);
    }
}