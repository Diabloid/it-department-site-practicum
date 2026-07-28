namespace it_department_site.Repositories.Interfaces
{
    public interface IDisciplineService
    {
        Task<IEnumerable<Discipline>> GetAllAsync();
        Task<Discipline?> GetByIdAsync(int id);
        Task CreateAsync(Discipline discipline);
        Task UpdateAsync(int id, Discipline discipline);
        Task DeleteAsync(int id);
    }
}
