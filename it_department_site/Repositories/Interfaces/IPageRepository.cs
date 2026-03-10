namespace it_department_site.Repositories.Interfaces
{
    public interface IPageRepository
    {
        Task<Page?> GetBySlugAsync(string slug);
        Task<Page?> GetByIdAsync(int id);
        Task<List<Page>> GetAllAsync();
        Task<int> CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(int id);

        Task<List<Page>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> GetTotalAsync(int pageSize);

        Task<bool> HasPageViewAsync(int userId, int pageId);
        Task AddPageViewAsync(int userId, int pageId);
        Task IncrementViewsAsync(int pageId);
        Task<int> GetViewsAsync(int pageId);
    }
}