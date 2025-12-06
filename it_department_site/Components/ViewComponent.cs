using Microsoft.AspNetCore.Mvc;
using it_department_site.Database;
using it_department_site.Services; // ✅ Тепер використовуємо Services

namespace it_department_site.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;

        public MenuViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IViewComponentResult Invoke()
        {
            var menuItems = _menuService.GetAllMenuItems();

            var lookup = menuItems.ToDictionary(m => m.Id);
            var rootItems = new List<MenuItem>();

            foreach (var item in menuItems)
            {
                if (item.ParentId.HasValue && lookup.ContainsKey(item.ParentId.Value))
                {
                    lookup[item.ParentId.Value].Children.Add(item);
                }
                else
                {
                    rootItems.Add(item);
                }
            }

            return View("_MenuPartial", rootItems);
        }
    }
}