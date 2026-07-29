namespace it_department_site.Services.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuItem>> GetAllMenuItemsAsync();
        Task<List<MenuItem>> GetRootMenuItemsAsync();
        Task LinkPageToMenuAsync(int id, int? parentMenuItemId, string title);
        Task<MenuItem?> GetMenuItemByIdAsync(int id);
        Task CreateMenuItemAsync(MenuItem item);
        Task UpdateMenuItemAsync(MenuItem item);
        Task DeleteMenuItemAsync(int id);
    }
}