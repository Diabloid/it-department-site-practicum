namespace it_department_site.Services.Interfaces
{
    public interface IDisciplineService
    {
        Task<IEnumerable<Discipline>> GetAllAsync();
        Task<IEnumerable<Discipline>> GetAllAsync(bool includeRequired);
        Task<Discipline?> GetByIdAsync(int id);
        Task<List<Discipline>> GetPagedAsync(int pageNumber, int pageSize, string? name);
        Task CreateAsync(Discipline discipline);
        Task UpdateAsync(int id, Discipline discipline);
        Task DeleteAsync(int id);
    }
}
