namespace it_department_site.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;

        public MenuViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menuItems = await _menuService.GetAllMenuItemsAsync();

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