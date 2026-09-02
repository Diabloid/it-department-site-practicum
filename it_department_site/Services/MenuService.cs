using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class MenuService(ApplicationDbContext dbContext) : IMenuService
    {
        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            return await dbContext.MenuItems
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<MenuItem>> GetMenuItemsTreeAsync()
        {
            return await dbContext.Set<MenuItem>()
                .Include(m => m.Page)
                .Include(m => m.Children.Where(c => !c.Disabled).OrderBy(c => c.SortOrder))
                .Where(m => m.ParentId == null && !m.Disabled)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task LinkPageToMenuAsync(int pageId, int? parentId, string title)
        {
            var existingItem = await dbContext.Set<MenuItem>()
                .FirstOrDefaultAsync(m => m.PageId == pageId);

            if (existingItem != null)
            {
                existingItem.Name = title;
                existingItem.ParentId = parentId;
            }
            else
            {
                var newItem = new MenuItem
                {
                    Name = title,
                    ParentId = parentId,
                    PageId = pageId,
                    SortOrder = 0
                };
                dbContext.Set<MenuItem>().Add(newItem);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
        {
            return await dbContext.Set<MenuItem>().FindAsync(id);
        }

        public async Task CreateMenuItemAsync(MenuItem item)
        {
            if (item.Type == MenuItemType.Separator)
            {
                item.Name = string.Empty;
            }

            dbContext.Set<MenuItem>().Add(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateMenuItemAsync(MenuItem item)
        {
            if (item.Type == MenuItemType.Separator)
            {
                item.Name = string.Empty;
            }

            dbContext.Set<MenuItem>().Update(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var item = await dbContext.Set<MenuItem>().FindAsync(id);
            if (item != null)
            {
                dbContext.Set<MenuItem>().Remove(item);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
