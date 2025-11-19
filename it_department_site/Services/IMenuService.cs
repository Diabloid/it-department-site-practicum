using it_department_site.Database;

namespace it_department_site.Services
{
    public interface IMenuService
    {
        List<MenuItem> GetAllMenuItems();
        void LinkPageToMenu(int id, int? parentMenuItemId, string title);
        MenuItem GetMenuItemById(int id);
        void CreateMenuItem(MenuItem item);
        void UpdateMenuItem(MenuItem item);
        void DeleteMenuItem(int id);
    }
}
