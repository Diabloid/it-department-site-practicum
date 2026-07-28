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
            var rootMenuItems = await _menuService.GetRootMenuItemsAsync();
            return View("_MenuPartial", rootMenuItems);
        }
    }
}