using it_department_site.Database;
using it_department_site.Repositories;

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
    }
}
