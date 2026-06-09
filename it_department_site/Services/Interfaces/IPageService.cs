namespace it_department_site.Services.Interfaces
{
    public interface IPageService
    {
        Task<Page?> GetPageBySlugAsync(string slug, bool isAdmin);
        Task<Page?> GetPageByIdAsync(int id);
        Task<List<Page>> GetAllPagesAsync();
        Task<int> CreatePageAsync(Page page);
        Task UpdatePageAsync(Page page);
        Task DeletePageAsync(int id);
        Task<List<Page>> GetPagedAsync(int b, int c);
        Task<int> GetTotalAsync(int p);

        Task IncrementViewsAsync(int pageId);
        Task<int> GetViewsAsync(int pageId);
    }
}