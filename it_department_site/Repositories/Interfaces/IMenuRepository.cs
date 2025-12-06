namespace it_department_site.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetAllAsync();
        Task LinkPageToMenuAsync(int pageId, int? parentId, string title);
        Task<MenuItem> GetByIdAsync(int id);
        Task CreateAsync(MenuItem item);
        Task UpdateAsync(MenuItem item);
        Task DeleteAsync(int id);
    }
}