namespace it_department_site.ViewComponents
{
    public class MenuViewComponent(IMenuService menuService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var rootMenuItems = await menuService.GetMenuItemsTreeAsync();
            return View("_MenuPartial", rootMenuItems);
        }
    }
}