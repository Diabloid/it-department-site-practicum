namespace it_department_site.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            return await _menuRepository.GetAllAsync();
        }

        public async Task LinkPageToMenuAsync(int pageId, int? parentId, string title)
        {
            await _menuRepository.LinkPageToMenuAsync(pageId, parentId, title);
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id) => await _menuRepository.GetByIdAsync(id);
        public async Task CreateMenuItemAsync(MenuItem item) => await _menuRepository.CreateAsync(item);
        public async Task UpdateMenuItemAsync(MenuItem item) => await _menuRepository.UpdateAsync(item);
        public async Task DeleteMenuItemAsync(int id) => await _menuRepository.DeleteAsync(id);
    }
}