namespace it_department_site.Services.Interfaces
{
    public interface IGraduateService
    {
        Task<IEnumerable<Graduate>> GetAllAsync();
        Task<Graduate?> GetByIdAsync(int id);
        Task CreateAsync(Graduate graduate);
        Task UpdateAsync(int id, Graduate graduate);
        Task DeleteAsync(int id);
        Task<IEnumerable<Graduate>> GetRandomReviewsAsync(int count);
    }
}