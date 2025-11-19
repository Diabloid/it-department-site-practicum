using it_department_site.Database;
using it_department_site.Repositories;
using ZstdSharp.Unsafe;

namespace it_department_site.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public List<MenuItem> GetAllMenuItems()
        {
            return _menuRepository.GetAll();
        }
        public void LinkPageToMenu(int pageId, int? parentId, string title)
        {
            _menuRepository.LinkPageToMenu(pageId, parentId, title);
        }
        public MenuItem GetMenuItemById(int id) => _menuRepository.GetById(id);
        public void CreateMenuItem(MenuItem item) => _menuRepository.Create(item);
        public void UpdateMenuItem(MenuItem item) => _menuRepository.Update(item);
        public void DeleteMenuItem(int id) => _menuRepository.Delete(id);
    }
}
