using it_department_site.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;

        public MenuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            return await _context.MenuItems
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task LinkPageToMenuAsync(int pageId, int? parentId, string title)
        {
            var existingItem = await _context.MenuItems
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
                _context.MenuItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
        {
            return await _context.MenuItems.FindAsync(id);
        }

        public async Task CreateMenuItemAsync(MenuItem item)
        {
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMenuItemAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item != null)
            {
                _context.MenuItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}